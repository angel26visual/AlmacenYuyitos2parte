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
    /// Lógica de interacción para VerInformacionDelivery.xaml
    /// </summary>
    public partial class VerInformacionDelivery 
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        public VerInformacionDelivery(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            nomUsuario = usuario;
            DatosUsuarios();
            cargarCbo();
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
            VisualizarPedidoDelivery vpd = new VisualizarPedidoDelivery(nomUsuario);
            vpd.Show();
            this.Close();
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

        public void cargarCbo()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_ESTADELIVERY , DESCRIP_ESTADELIVERY FROM ESTADO_PED";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboEstado.ItemsSource = dt.AsDataView();
                cboEstado.DisplayMemberPath = "DESCRIP_ESTADELIVERY";
                cboEstado.SelectedValuePath = "ID_ESTADELIVERY";
            }
            catch (Exception)
            {


            }
        }

        private async void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_actualizar_delivery", con);
                cmd.CommandType = CommandType.StoredProcedure;

                if (dpFechaDePedido.SelectedDate != null)
                {
                    cmd.Parameters.Add("fecha_venta", OracleDbType.Date).Value = dpFechaDePedido.SelectedDate;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "La Fecha de Pedido debe ser válida");
                    return;

                }

                if (dpFechaDeEntrega.SelectedDate != null)
                {
                    cmd.Parameters.Add("fecha_entrega", OracleDbType.Date).Value = dpFechaDeEntrega.SelectedDate;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "La Fecha de Pedido debe ser válida");
                    return;

                }



                if (txtNombreDeCliente.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("nom_cliente", OracleDbType.Varchar2, 100).Value = txtNombreDeCliente.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Nombre del Cliente debe contener 3 o más caracteres!");
                    return;

                }

                if (txtDireccion.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("direccion", OracleDbType.Varchar2, 100).Value = txtDireccion.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El la dirección del cliente debe contener 3 o más caracteres!");
                    return;

                }

                if (txtTelefonoContacto.Text.Replace(" ", string.Empty).Length == 9)
                {
                    cmd.Parameters.Add("fono", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtTelefonoContacto.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Teléfono del cliente debe contener 9 dígitos!");
                    return;

                }

                if (txtTotalDescuentos.Text.Replace(" ", string.Empty).Length >= 1)
                {
                    cmd.Parameters.Add("descuento", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtTotalDescuentos.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El DESCUENTO TOTAL DEBE CONTENER MINIMO UN DÍGITO!");
                    return;

                }

                if (txtMontoTotal.Text.Replace(" ", string.Empty).Length >= 1)
                {
                    cmd.Parameters.Add("monto_total", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtMontoTotal.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El monto total debe contener al menos un dígito!");
                    return;

                }

                if (txtMontoFinal.Text.Replace(" ", string.Empty).Length >= 1)
                {
                    cmd.Parameters.Add("monto_final", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtMontoFinal.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El monto FINAL debe contener al menos un dígito!");
                    return;

                }

                if (cboEstado.SelectedValue != null)
                { cmd.Parameters.Add("id_estadoP", OracleDbType.Int32, 20).Value = cboEstado.SelectedValue; }
                else { await this.ShowMessageAsync("Error", "debe seleccionar un estado!"); return; }


                cmd.Parameters.Add("nro_boleta", OracleDbType.Varchar2, 100).Value = txtNumeroDeBoleta.Text;

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n < 0)
                    {
                        await this.ShowMessageAsync("actualizado", "Delivery actualizado correctamente");
                        this.resetAll();


                    }
                }
                catch (Exception ep)
                {
                    await this.ShowMessageAsync("Error", ep.ToString());
                    return;
                }

            }
            catch (Exception)
            {


            }
        }

        public void resetAll()
        {
            txtNumeroDeBoleta.Text = "";
            txtMontoTotal.Text = "";
            txtMontoFinal.Text = "";
            txtNombreDeCliente.Text = "";
            txtDireccion.Text = "";
            txtTelefonoContacto.Text = "";
            txtTotalDescuentos.Text = "";

        }

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("NRO_BOLETA", OracleDbType.Varchar2, 100).Value = txtNumeroDeBoleta.Text;
                cmd.CommandText = "delete from boleta where nro_boleta = :NRO_BOLETA";

                MessageDialogResult respuesta = await this.ShowMessageAsync("ELIMINAR", "¿Desea Eliminar Información del Delivery Seleccionado?", MessageDialogStyle.AffirmativeAndNegative);

                if (respuesta == MessageDialogResult.Affirmative)
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("eliminado", "Delivery eliminado correctamente");
                        this.resetAll();

                    }
                }
                else
                {
                    return;
                }



            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
