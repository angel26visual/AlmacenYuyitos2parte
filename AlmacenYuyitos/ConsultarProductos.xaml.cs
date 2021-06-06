using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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
    /// Lógica de interacción para ConsultarProductos.xaml
    /// </summary>
    public partial class ConsultarProductos
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        public ConsultarProductos(string usuario)
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

        private async void setConnection()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            con = new OracleConnection(connectionString);
            try
            {
                con.Open();
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "No hay conexión con la base de datos");
            }
        }

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            GestionarVentas gv = new GestionarVentas(nomUsuario);
            gv.Show();
            this.Close();
        }

        private async void txtCodigoP_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                String sql = "SELECT CODIGO_PRODUCTO, IMG_PRODUC, NOMBRE_PRODUCT, PRECIO_VENTA, STOCK, MARCA, COD_BARRA_PRODUCT FROM PRODUCTO WHERE COD_BARRA_PRODUCT = :CODIGO";
                this.AUD(sql, 0);
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
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
                    cmd.Parameters.Add("CODIGO", OracleDbType.Varchar2, 20).Value = txtCodigoP.Text;
                    try
                    {
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgProductos.ItemsSource = dt.DefaultView;
                            reader.Close();
                        }
                        else
                        {
                            dgProductos.ItemsSource = null;
                        }
                    }
                    catch (Exception)
                    {
                        await this.ShowMessageAsync("Error", "Ha ocurrido un error");
                    }
                    break;
                case 1:
                    cmd.Parameters.Add("NOMBRE", OracleDbType.Varchar2, 50).Value = txtNombreProducto.Text;
                    try
                    {
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgProductos.ItemsSource = dt.DefaultView;
                            reader.Close();
                        }
                        else
                        {
                            dgProductos.ItemsSource = null;
                            await this.ShowMessageAsync("Productos", "No hay productos que coinicidan con el nombre ingresado");
                        }
                    }
                    catch (Exception e)
                    {
                        await this.ShowMessageAsync("Error", "Ha ocurrido un error");
                    }

                    break;

            }
        }

        private async void btnFiltrarNombreProducto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String sql = "SELECT CODIGO_PRODUCTO, IMG_PRODUC, NOMBRE_PRODUCT, PRECIO_VENTA, STOCK, MARCA, COD_BARRA_PRODUCT FROM PRODUCTO WHERE LOWER(NOMBRE_PRODUCT) LIKE LOWER('%' || :NOMBRE || '%')";
                this.AUD(sql, 1);
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
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

        private async void txtCodigoP_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Código del Producto debe contener sólo números");
            }
        }
    }
}

