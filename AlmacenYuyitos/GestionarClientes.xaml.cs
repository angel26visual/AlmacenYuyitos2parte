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
    /// Lógica de interacción para GestionarClientes.xaml
    /// </summary>
    public partial class GestionarClientes 
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        public GestionarClientes(string usuario)
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

        private void ActualizarDataGrid()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT RUT_CLI , NOMBRE_CLI , APELLIDO_CLI , FONO_CLI , CORREO_CLI " +
                    "FROM CLIENTE ORDER BY NOMBRE_CLI ASC";
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dgClientes.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }

        }

        private void btnVolverAlMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
            this.Close();
         
        }

 


        private async void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;

                if (ValidarRut(txtRutCliente.Text))
                { cmd.Parameters.Add("RUT_CLI", OracleDbType.Varchar2, 100).Value = txtRutCliente.Text; }
                else
                {
                    await this.ShowMessageAsync("Error", "debe ingresar un rut valido!");
                    return;

                }

                if (txtNombreCliente.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("NOMBRE_CLI", OracleDbType.Varchar2, 100).Value = txtNombreCliente.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el nombre del/la cliente(a) debe tener mas de 3 caracteres!");
                    return;

                }

                if (txtApellidoCliente.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("APELLIDO_CLI", OracleDbType.Varchar2, 100).Value = txtApellidoCliente.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el Apellido del/la cliente(a) debe tener mas de 3 caracteres!");
                    return;

                }
                if (txtTelefonoCliente.Text.Replace(" ", string.Empty).Length == 9)
                {
                    cmd.Parameters.Add("FONO_CLI", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtTelefonoCliente.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el Teléfono debe tener 9 dígitos!");
                    return;
                }

                if (txtCorreoCliente.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("CORREO_CLI", OracleDbType.Varchar2, 100).Value = txtCorreoCliente.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el correo debe tener mas de 3 caracteres!");
                    return;

                }

                cmd.CommandText = "update CLIENTE set rut_cli =:RUT_CLI,nombre_cli=:NOMBRE_CLI,apellido_cli=:APELLIDO_CLI,fono_cli=:FONO_CLI,correo_cli=:CORREO_CLI where rut_cli=:RUT_CLI";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("actualizado", "Cliente actualizado correctamente");
                        this.ActualizarDataGrid();

                    }
                }
                catch (Exception)
                {
                    await this.ShowMessageAsync("Error", "no se pudo actualizar");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void resetAll()
        {
            txtRutCliente.Text = "";
            txtNombreCliente.Text = "";
            txtApellidoCliente.Text = "";
            txtTelefonoCliente.Text = "";
            txtCorreoCliente.Text = "";


            btnAgregarCliente.IsEnabled = true;
            btnModificar.IsEnabled = false;
            btnEliminarCliente.IsEnabled = false;
        }

        private async void btnEliminarCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("RUT_CLI", OracleDbType.Varchar2, 100).Value = txtRutCliente.Text;
                cmd.CommandText = "delete from CLIENTE where rut_cli = :RUT_CLI";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("eliminado", "Cliente eliminado correctamente");
                        this.ActualizarDataGrid();
                        this.resetAll();

                    }
                }
                catch (Exception)
                {
                    await this.ShowMessageAsync("Error", "no se pudo eliminar");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            this.resetAll();

        }

        private void dgClientes_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActualizarDataGrid();
        }

        private void dgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                txtRutCliente.Text = dr["RUT_CLI"].ToString();
                txtNombreCliente.Text = dr["NOMBRE_CLI"].ToString();
                txtApellidoCliente.Text = dr["APELLIDO_CLI"].ToString();
                txtTelefonoCliente.Text = dr["FONO_CLI"].ToString();
                txtCorreoCliente.Text = dr["CORREO_CLI"].ToString();

                btnAgregarCliente.IsEnabled = false;
                btnModificar.IsEnabled = true;
                btnEliminarCliente.IsEnabled = true;

            }
        }

        private void btnVolverAlMenu_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
            this.Close();
        }

        private async void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.BindByName = true;

                if (ValidarRut(txtRutCliente.Text))
                { cmd.Parameters.Add("RUT_CLI", OracleDbType.Varchar2, 100).Value = txtRutCliente.Text; }
                else
                {
                    await this.ShowMessageAsync("Error", "debe ingresar un rut valido!");
                    return;

                }

                if (txtNombreCliente.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("NOMBRE_CLI", OracleDbType.Varchar2, 100).Value = txtNombreCliente.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el nombre del/la cliente(a) debe tener mas de 3 caracteres!");
                    return;

                }

                if (txtApellidoCliente.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("APELLIDO_CLI", OracleDbType.Varchar2, 100).Value = txtApellidoCliente.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el Apellido del/la cliente(a) debe tener mas de 3 caracteres!");
                    return;

                }
                if (txtTelefonoCliente.Text.Replace(" ", string.Empty).Length == 9)
                {
                    cmd.Parameters.Add("FONO_CLI", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtTelefonoCliente.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el Teléfono debe tener 9 dígitos!");
                    return;
                }

                if (txtCorreoCliente.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("CORREO_CLI", OracleDbType.Varchar2, 100).Value = txtCorreoCliente.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el correo debe tener mas de 3 caracteres!");
                    return;

                }

                

                cmd.CommandText = "INSERT INTO CLIENTE(RUT_CLI , NOMBRE_CLI, APELLIDO_CLI , FONO_CLI , CORREO_CLI) " +
                                   "VALUES(:RUT_CLI,:NOMBRE_CLI ,:APELLIDO_CLI , :FONO_CLI , :CORREO_CLI)";
                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("Agregado", "Cliente se agregó correctamente");
                        this.ActualizarDataGrid();
                        resetAll();
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }


            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("Error", e.ToString());
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

        private void btnDeudas_Click(object sender, RoutedEventArgs e)
        {
            VisualizarDeudas vd = new VisualizarDeudas(txtRutCliente.Text, nomUsuario);

            vd.Show();
            this.Close();
        }

        public bool ValidarRut(string rut)
        {

            bool validacion = false;
            try
            {
                rut = rut.ToUpper();
                rut = rut.Replace(".", "");
                rut = rut.Replace("-", "");
                int rutAux = int.Parse(rut.Substring(0, rut.Length - 1));

                char dv = char.Parse(rut.Substring(rut.Length - 1, 1));

                int m = 0, s = 1;
                for (; rutAux != 0; rutAux /= 10)
                {
                    s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
                }
                if (dv == (char)(s != 0 ? s + 47 : 75))
                {
                    validacion = true;
                }
            }
            catch (Exception)
            {
            }
            return validacion;
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

        private async void txtNombreCliente_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[a-zA-Z]"))
            {
                e.Handled = true;
                await this.ShowMessageAsync("Error", "El Nombre del Cliente debe contener sólo letras");
            }
        }

        private async void txtApellidoCliente_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[a-zA-Z]"))
            {
                e.Handled = true;
                await this.ShowMessageAsync("Error", "El Apellido del cliente debe contener sólo letras");
            }
        }
    }
}
