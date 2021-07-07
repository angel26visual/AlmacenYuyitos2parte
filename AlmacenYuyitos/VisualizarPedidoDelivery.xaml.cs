using BibliotecaLosYuyitos;
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
using System.Data;
using MahApps.Metro.Controls.Dialogs;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para VisualizarPedidoDelivery.xaml
    /// </summary>
    public partial class VisualizarPedidoDelivery 
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        string commandtable = " SELECT NRO_BOLETA,FECHA_VENTA , FECHA_ENTREGA ,MONTO_TOTAL,NOM_CLIENTE , DIRECCION_CLIENTE , FONO_CONTACTO ,TOTAL_DESCUENTOS , TOTAL_VENTA , MONTO_PAGO , ESTADO_PED_ID_ESTADELIVERY FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 3";
        public VisualizarPedidoDelivery(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            nomUsuario = usuario;
            DatosUsuarios();
            actualizarDataGrid();

        }

        private void actualizarDataGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = commandtable;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dgPedidosDelivery.ItemsSource = dt.DefaultView;
            dr.Close();
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

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarDelivery gd = new GestionarDelivery(nomUsuario);
            gd.Show();
            this.Close();
        }

        private void btnVerInfoDelivery_Click(object sender, RoutedEventArgs e)
        {
            DataRowView datos = dgPedidosDelivery.SelectedItem as DataRowView;
            if (datos != null)
            {
                VerInformacionDelivery vid = new VerInformacionDelivery(nomUsuario, int.Parse(datos["MONTO_TOTAL"].ToString()), int.Parse(datos["TOTAL_DESCUENTOS"].ToString()), int.Parse(datos["NRO_BOLETA"].ToString()));
                vid.txtNumeroBoleta.Text = datos["NRO_BOLETA"].ToString();
                vid.txtFechaVenta.Text = datos["FECHA_VENTA"].ToString();
                vid.dpFechaeEntrega.Text = datos["FECHA_ENTREGA"].ToString();
                vid.txtNombreCliente.Text = datos["NOM_CLIENTE"].ToString();
                vid.txtDireccionDelivery.Text = datos["DIRECCION_CLIENTE"].ToString();
                vid.txtTelefonoContacto.Text = datos["FONO_CONTACTO"].ToString();
                vid.txtTotalDescuentos.Text = datos["TOTAL_DESCUENTOS"].ToString();
                vid.txtTotalDelivery.Text = datos["MONTO_TOTAL"].ToString();
                vid.cboEstado.SelectedValue = int.Parse(datos["ESTADO_PED_ID_ESTADELIVERY"].ToString());
                vid.txtValorDespacho.Text = (int.Parse(datos["TOTAL_VENTA"].ToString()) - (int.Parse(datos["MONTO_TOTAL"].ToString()) + int.Parse(datos["TOTAL_DESCUENTOS"].ToString()))).ToString();
                vid.txtPago.Text = datos["MONTO_PAGO"].ToString();
                vid.Show();
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

        private void btnFiltrarFechaDelivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String sql = " SELECT NRO_BOLETA,FECHA_VENTA , FECHA_ENTREGA ,MONTO_TOTAL,NOM_CLIENTE , DIRECCION_CLIENTE , FONO_CONTACTO ,TOTAL_DESCUENTOS , TOTAL_VENTA , MONTO_PAGO , ESTADO_PED_ID_ESTADELIVERY FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 3 and FECHA_VENTA >= :FECHA_VENTA";
                this.AUD(sql,0);
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async void AUD(String sql_stmt, int state)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;

            switch (state)
            {
                case 0:
                    cmd.Parameters.Add("FECHA_VENTA", OracleDbType.Date).Value = dpFechaPedidoDelivery.SelectedDate;
                    try
                    {
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgPedidosDelivery.ItemsSource = dt.DefaultView;
                            reader.Close();
                        }
                        else
                        {
                            dgPedidosDelivery.ItemsSource = null;
                            await this.ShowMessageAsync("PEDIDOS DELIVERY", "No hay venta de pedidos delivery en la fecha seleccionada ");
                        }
                    }
                    catch (Exception)
                    {
                        await this.ShowMessageAsync("Error", "Ha ocurrido un error");
                    }
                    break;
                case 1:
                    cmd.Parameters.Add("FECHA_ENTREGA", OracleDbType.Date).Value = dpFechaEntregaDelivery.SelectedDate;
                    try
                    {
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgPedidosDelivery.ItemsSource = dt.DefaultView;
                            reader.Close();
                        }
                        else
                        {
                            dgPedidosDelivery.ItemsSource = null;
                            await this.ShowMessageAsync("PEDIDOS DELIVERY", "No hay Pedidos por entregar en la fecha seleccionada ");
                        }
                    }
                    catch (Exception)
                    {
                        await this.ShowMessageAsync("Error", "Ha ocurrido un error");
                    }
                    break;

            }
        }

        private void btnFiltrarFechaEntrega_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String sql = " SELECT NRO_BOLETA,FECHA_VENTA , FECHA_ENTREGA ,MONTO_TOTAL,NOM_CLIENTE , DIRECCION_CLIENTE , FONO_CONTACTO ,TOTAL_DESCUENTOS , TOTAL_VENTA , MONTO_PAGO , ESTADO_PED_ID_ESTADELIVERY FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 3 and FECHA_ENTREGA = :FECHA_";
                this.AUD(sql, 1);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
