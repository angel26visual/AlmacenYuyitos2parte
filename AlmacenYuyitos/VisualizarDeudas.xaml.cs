using System;
using System.Collections.Generic;
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
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using MahApps.Metro.Controls.Dialogs;
using System.Data;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para VisualizarDeudas.xaml
    /// </summary>
    public partial class VisualizarDeudas
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        public VisualizarDeudas(string rut, string usuario)
        {
            InitializeComponent();
            setConnection();
            ActualizarDataGrid(rut);
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

        private void ActualizarDataGrid(string rut)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.Parameters.Add("RUT", OracleDbType.Varchar2, 100).Value = rut;
                cmd.CommandText = "SELECT cliente_rut_cli ,nro_boleta ,tipo_venta_id_tipventa ,fecha_venta ,monto_total , monto_pago , monto_total-monto_pago montoadeudado , estado_deuda_id_estadeuda from boleta " +
                    "" +
                    "where tipo_venta_id_tipventa = 2 and cliente_rut_cli = :RUT";
                
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dtgDeudas.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }

        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarClientes myec = new GestionarClientes(nomUsuario);
            myec.Show();
            this.Close();
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

        private void btnVerInformacion_Click(object sender, RoutedEventArgs e)
        {
            VerInformacion vi = new VerInformacion(nomUsuario);
            DataRowView datos = dtgDeudas.SelectedItem as DataRowView;
            if (datos != null)
            {
                vi.txtRutlienteDeuda.Text = datos["CLIENTE_RUT_CLI"].ToString();
                vi.txtNumeroDeBoleta.Text = datos["NRO_BOLETA"].ToString();
                vi.txtTipoVenta.Text = datos["TIPO_VENTA_ID_TIPVENTA"].ToString();
                vi.dpFechaDeuda.Text = datos["FECHA_VENTA"].ToString();
                vi.txtMontoTotal.Text = datos["MONTO_TOTAL"].ToString();
                vi.txtMontoPagado.Text = datos["MONTO_PAGO"].ToString();
                vi.cboEstado.SelectedValue = int.Parse(datos["ESTADO_DEUDA_ID_ESTADEUDA"].ToString());


                vi.Show();
                this.Close();
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
    }
}
