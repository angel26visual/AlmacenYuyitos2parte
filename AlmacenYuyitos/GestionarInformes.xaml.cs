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
    /// Lógica de interacción para GestionarInformes.xaml
    /// </summary>
    public partial class GestionarInformes
    {

        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        int clientesR = 0;
        int clientesDI = 0;
        int clientesSD = 0;
        int clientesDP = 0;
        int usuariosR = 0;
        int usuariosA = 0;
        int usuariosV = 0;
        public GestionarInformes(string usuario)
        {
            this.setConnection();
            InitializeComponent();
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

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
            this.Close();
        }

        private void btnTodos_Click(object sender, RoutedEventArgs e)
        {
            checkClientes.IsChecked = true;
            checkUsuarios.IsChecked = true;
            checkOrdenPedidos.IsChecked = true;
            checkGestionVentas.IsChecked = true;
            checkGestionProductos.IsChecked = true;
            checkPagos.IsChecked = true;
            checkPromociones.IsChecked = true;
            checkProveedores.IsChecked = true;
            checkDelivery.IsChecked = true;
            checkRecepcion.IsChecked = true;
        }

        private void btnNinguno_Click(object sender, RoutedEventArgs e)
        {
            checkClientes.IsChecked = false;
            checkUsuarios.IsChecked = false;
            checkOrdenPedidos.IsChecked = false;
            checkGestionVentas.IsChecked = false;
            checkGestionProductos.IsChecked = false;
            checkPagos.IsChecked = false;
            checkPromociones.IsChecked = false;
            checkProveedores.IsChecked = false;
            checkDelivery.IsChecked = false;
            checkRecepcion.IsChecked = false;
        }

        private async void btnGenerarInforme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkClientes.IsChecked == true)
                {
                    ComprobarClientesR();
                    ComprobarClienesDI();
                    ComrpobarClientesSD();
                    ComprobarClientesDP();
                }
                if (checkUsuarios.IsChecked == true)
                {

                }

                VistaInforme informe = new VistaInforme(nomUsuario);
                informe.lbClientesR.Content = clientesR;
                informe.lbClientesD.Content = clientesDI;
                informe.lbClientesSD.Content = clientesSD;
                informe.lbClientesDP.Content = clientesDP;
                informe.Show();
                this.Close();
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarClientesR()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS CLIENTES FROM CLIENTE";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    clientesR = int.Parse(reader["CLIENTES"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("CLIENTES", "No se ha podido obtener la información de los clientes");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarClienesDI()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(DISTINCT(CLIENTE_RUT_CLI)),0) AS IMPAGAS FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 2 AND ESTADO_DEUDA_ID_ESTADEUDA = 1";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    clientesDI = int.Parse(reader["IMPAGAS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("CLIENTES", "No se ha podido obtener la información de los clientes");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComrpobarClientesSD()
        {
            try
            {

            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarClientesDP()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(DISTINCT(CLIENTE_RUT_CLI)),0) AS PAGADAS FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 2 AND ESTADO_DEUDA_ID_ESTADEUDA = 2";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    clientesDP = int.Parse(reader["PAGADAS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("CLIENTES", "No se ha podido obtener la información de los clientes");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComrpobarUsuariosR()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS USUARIOS FROM TRABAJADOR";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    usuariosR = int.Parse(reader["USUARIOS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("USUARIOS", "No se ha podido obtener la información de los usuarios");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }
    }
}
