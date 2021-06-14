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
        string commandtable = " SELECT NRO_BOLETA,FECHA_VENTA , FECHA_ENTREGA ,MONTO_TOTAL,NOM_CLIENTE , DIRECCION_CLIENTE , FONO_CONTACTO ,TOTAL_DESCUENTOS , TOTAL_VENTA , ESTADO_PED_ID_ESTADELIVERY FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 3";
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
            

            VerInformacionDelivery vid = new VerInformacionDelivery(nomUsuario);
            DataRowView datos = dgPedidosDelivery.SelectedItem as DataRowView;
            if (datos != null)
            {
                vid.txtNumeroDeBoleta.Text = datos["NRO_BOLETA"].ToString();
                vid.dpFechaDePedido.Text = datos["FECHA_VENTA"].ToString();
                vid.dpFechaDeEntrega.Text = datos["FECHA_ENTREGA"].ToString();
                vid.txtNombreDeCliente.Text = datos["NOM_CLIENTE"].ToString();
                vid.txtDireccion.Text = datos["DIRECCION_CLIENTE"].ToString();
                vid.txtTelefonoContacto.Text = datos["FONO_CONTACTO"].ToString();
                vid.txtTotalDescuentos.Text = datos["TOTAL_DESCUENTOS"].ToString();
                vid.txtMontoTotal.Text = datos["MONTO_TOTAL"].ToString();
                vid.txtMontoFinal.Text = datos["TOTAL_VENTA"].ToString();
                vid.cboEstado.SelectedValue = int.Parse(datos["ESTADO_PED_ID_ESTADELIVERY"].ToString());




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
    }
}
