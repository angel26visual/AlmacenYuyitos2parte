using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para VisualizarVentas.xaml
    /// </summary>
    public partial class VisualizarVentas 
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        string commandtable = "SELECT NRO_BOLETA, FECHA_VENTA, TOTAL_VENTA, TOTAL_DESCUENTOS, MONTO_TOTAL, MONTO_PAGO, m.DESCRIP_MEDIOPAGO AS MEDIO_PAGO, TRABAJADOR_RUT_TRAB AS RUT_TRABAJADOR, NVL(CLIENTE_RUT_CLI, 'SIN REGISTRO') AS RUT_CLIENTE, NVL(NOM_CLIENTE, 'SIN REGISTRO') AS CLIENTE, NVL(ESTADO_DEUDA_ID_ESTADEUDA, 0) AS ESTADO FROM BOLETA b INNER JOIN MEDIO_PAGO m ON MEDIO_PAGO_ID_MEDIOPAGO = ID_MEDIOPAGO WHERE TIPO_VENTA_ID_TIPVENTA = 1 OR TIPO_VENTA_ID_TIPVENTA = 2"; 
        public VisualizarVentas(string usuario)
        {
            setConnection();
            InitializeComponent();
            actualizarDataGrid();
            nomUsuario = usuario;
            DatosUsuarios();
        }

        private async void DatosUsuarios()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT NOMBRE_TRAB, APELLIDO_TRAB, CARGO_TRABAJADOR_ID_CARGO FROM TRABAJADOR WHERE NOM_USUARIO = :USUARIO";
            cmd.CommandType = CommandType.Text;
            try
            {
                cmd.Parameters.Add("USUARIO", OracleDbType.Varchar2, 100).Value = nomUsuario.ToString();
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    btnCuenta.Content = "Bienvenido/a " + reader["NOMBRE_TRAB"] + " " + reader["APELLIDO_TRAB"];
                    cargo = int.Parse(reader["CARGO_TRABAJADOR_ID_CARGO"].ToString());
                    nombre = reader["NOMBRE_TRAB"].ToString();
                    apellido = reader["APELLIDO_TRAB"].ToString();
                }
                else
                {
                    await this.ShowMessageAsync("Información de contacto", "No se a podido traer la información del usuario");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private void setConnection()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            con = new OracleConnection(connectionString);

            try
            {
                con.Open();
            }
            catch (Exception exp) { }
        }
        private void actualizarDataGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = commandtable;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dgVenta.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarVentas gv = new GestionarVentas(nomUsuario);
            gv.Show();
            this.Close();
        }

        private async void btnVer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView dataRow = dgVenta.SelectedItem as DataRowView;
                if (dataRow != null)
                {
                    int medioPago = ConseguirIdMedioP(dataRow["MEDIO_PAGO"].ToString());
                    VerVenta venta = new VerVenta(nomUsuario, Convert.ToInt32(dataRow["NRO_BOLETA"].ToString()));
                    venta.Show();
                    venta.txtNroBoleta.Text = dataRow["NRO_BOLETA"].ToString();
                    venta.txtFechaVenta.SelectedDate = Convert.ToDateTime(dataRow["FECHA_VENTA"].ToString());
                    venta.cboMedioPago.SelectedValue = medioPago;
                    venta.txtPago.Text = dataRow["MONTO_PAGO"].ToString();
                    venta.txtTotalDescuento.Text = dataRow["TOTAL_DESCUENTOS"].ToString();
                    venta.txtTotalVenta.Text = dataRow["MONTO_TOTAL"].ToString();
                    venta.monto_total = Convert.ToInt32(dataRow["MONTO_TOTAL"].ToString());
                    if (Convert.ToInt32(dataRow["ESTADO"].ToString()) != 0)
                    {
                        venta.checkFiado.IsChecked = true;
                        venta.txtRutCli.Text = dataRow["RUT_CLIENTE"].ToString();
                        venta.txtNombreCliente.Text = dataRow["CLIENTE"].ToString();
                        venta.verifiCli = 1;
                    }
                    else
                    {
                        venta.checkFiado.IsChecked = false;
                    }
                    this.Close();
                    await this.ShowMessageAsync("Venta", "Información de la venta obtenida correctamente");

                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
            
        }

        private void btnCuenta_Click(object sender, RoutedEventArgs e)
        {
            if (cuentaFlyouts.IsOpen == true)
            {
                cuentaFlyouts.IsOpen = false;
            }
            else
            {
                cuentaFlyouts.IsOpen = true;
            }
        }

        private async void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MessageDialogResult respuesta = await this.ShowMessageAsync("Cerrar Sesión", "¿Desea cerrar Sesión?", MessageDialogStyle.AffirmativeAndNegative);

            if (respuesta == MessageDialogResult.Affirmative)
            {
                await this.ShowMessageAsync("Éxito", "Usted ha cerrado sesión exitosamente");
                Login log = new Login();
                log.Show();
                this.Close();
            }
            else
            {
                return;
            }
        }

        private void btnFiltrarFechaVenta_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT NRO_BOLETA, FECHA_VENTA, TOTAL_VENTA, TOTAL_DESCUENTOS, MONTO_TOTAL, MONTO_PAGO, m.DESCRIP_MEDIOPAGO AS MEDIO_PAGO, TRABAJADOR_RUT_TRAB AS RUT_TRABAJADOR, NVL(CLIENTE_RUT_CLI, 'SIN REGISTRO') AS RUT_CLIENTE, NVL(NOM_CLIENTE, 'SIN REGISTRO') AS CLIENTE, NVL(ESTADO_DEUDA_ID_ESTADEUDA, 0) AS ESTADO FROM BOLETA b INNER JOIN MEDIO_PAGO m ON MEDIO_PAGO_ID_MEDIOPAGO = ID_MEDIOPAGO WHERE FECHA_VENTA = :FECHA_VENTA AND (TIPO_VENTA_ID_TIPVENTA = 1 OR TIPO_VENTA_ID_TIPVENTA = 2)";
            cmd.Parameters.Add("FECHA_VENTA", OracleDbType.Varchar2, 100).Value = txtFechaDeVenta.Text;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dgVenta.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private void btnFiltrarTipoVenta_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT NRO_BOLETA, FECHA_VENTA, TOTAL_VENTA, TOTAL_DESCUENTOS, MONTO_TOTAL, MONTO_PAGO, m.DESCRIP_MEDIOPAGO AS MEDIO_PAGO, TRABAJADOR_RUT_TRAB AS RUT_TRABAJADOR, NVL(CLIENTE_RUT_CLI, 'SIN REGISTRO') AS RUT_CLIENTE, NVL(NOM_CLIENTE, 'SIN REGISTRO') AS CLIENTE, NVL(ESTADO_DEUDA_ID_ESTADEUDA, 0) AS ESTADO FROM BOLETA b INNER JOIN MEDIO_PAGO m ON MEDIO_PAGO_ID_MEDIOPAGO = ID_MEDIOPAGO WHERE NRO_BOLETA = :NRO_BOLETA AND (TIPO_VENTA_ID_TIPVENTA = 1 OR TIPO_VENTA_ID_TIPVENTA = 2)";
            cmd.Parameters.Add("NRO_BOLETA", OracleDbType.Varchar2, 100).Value = txtNroBoleta.Text;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dgVenta.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private int ConseguirIdMedioP(string descripcion)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ID_MEDIOPAGO FROM MEDIO_PAGO WHERE LOWER(DESCRIP_MEDIOPAGO) = :descripcion";
            cmd.Parameters.Add("descripcion", OracleDbType.Varchar2, 100).Value = descripcion.ToLower();
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                return Convert.ToInt32(dr["ID_MEDIOPAGO"].ToString());
            }
            return 0;
            
        }
    }
}
