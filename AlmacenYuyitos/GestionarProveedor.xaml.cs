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
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using MahApps.Metro.Controls.Dialogs;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para GestionarProveedor.xaml
    /// </summary>
    public partial class GestionarProveedor
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        public GestionarProveedor(string usuario)
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

        private void ActualizarDataGrid()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT RUT_PROVEE , NOMBRE_PROVEE , DIRECCION_PROVEE , TELEFONO_1_PROVEE , TELEFONO_2_PROVEE , NOM_SERVIDOR," +
                    "TELEFONO_SERVIDOR FROM PROVEEDOR ORDER BY NOMBRE_PROVEE ASC";
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dgProveedor.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }

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

        private  async void btnCerrarSesión_Click(object sender, RoutedEventArgs e)
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

        private void btnVolverAlMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
            this.Close();
        }




        private void dgProveedor_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActualizarDataGrid();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            con.Close();
        }

       

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("RUT_PROVEE", OracleDbType.Varchar2, 100).Value = txtRutProveedor.Text;
                cmd.CommandText = "delete from proveedor where rut_provee = :RUT_PROVEE";

                MessageDialogResult respuesta = await this.ShowMessageAsync("ELIMINAR", "¿Desea Eliminar Información del Proveedor Seleccionado?", MessageDialogStyle.AffirmativeAndNegative);
                if (respuesta == MessageDialogResult.Affirmative) { 
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("eliminado", "Proveedor eliminado correctamente");
                        this.resetAll();
                        this.ActualizarDataGrid();

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

        private async void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_actualizar_proveedor", con);
                cmd.CommandType = CommandType.StoredProcedure;

                if (ValidarRut(txtRutProveedor.Text))
                {
                    cmd.Parameters.Add("rut", OracleDbType.Varchar2, 100).Value = txtRutProveedor.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "debe ingresar un rut valido!");
                    btnRegistrar.IsEnabled = true;
                    return;


                }

                if (txtNombreProveedor.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("nombre", OracleDbType.Varchar2, 100).Value = txtNombreProveedor.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el nombre del/la proveedor(a) debe tener mas de 3 caracteres!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtDireccionProveedor.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("direccion", OracleDbType.Varchar2, 100).Value = txtDireccionProveedor.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "La Dirección debe tener mas de 3 caracteres!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtFonoProveedorUno.Text.Replace(" ", string.Empty).Length == 9)
                {
                    cmd.Parameters.Add("telefono_1", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtFonoProveedorUno.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Teléfono 1 del proveedor debe contener 9 dígitos!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtFonoProveedor2.Text.Replace(" ", string.Empty).Length == 9)
                {
                    cmd.Parameters.Add("telefono_2", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtFonoProveedor2.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Teléfono 2 del proveedor debe contener 9 dígitos!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtNombreServidor.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("nombre_s", OracleDbType.Varchar2, 100).Value = txtNombreServidor.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Nombre del proveedor debe contener 3 o más caracteres!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtTelefonoServidor.Text.Replace(" ", string.Empty).Length == 9)
                {
                    cmd.Parameters.Add("telefono_s", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtTelefonoServidor.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Teléfono  del servidor debe contener 9 dígitos!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n < 0)
                    {
                        await this.ShowMessageAsync("actualizado", "proveedor actualizado correctamente");
                        this.resetAll();
                        this.ActualizarDataGrid();

                    }
                }
                catch (Exception)
                {
                    await this.ShowMessageAsync("Error", "no se pudo actualizar");
                }
                this.resetAll();
            }
            catch (Exception)
            {


            }
        }
        private void resetAll()
        {
            txtRutProveedor.Text = "";
            txtNombreProveedor.Text = "";
            txtDireccionProveedor.Text = "";
            txtFonoProveedorUno.Text = "";
            txtFonoProveedor2.Text = "";
            txtNombreServidor.Text = "";
            txtTelefonoServidor.Text = "";

            btnRegistrar.IsEnabled = true;
            btnActualizar.IsEnabled = false;
            btnEliminar.IsEnabled = false;
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            this.resetAll();
        }

        

        private void dgProveedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                txtRutProveedor.Text = dr["RUT_PROVEE"].ToString();
                txtNombreProveedor.Text = dr["NOMBRE_PROVEE"].ToString();
                txtDireccionProveedor.Text = dr["DIRECCION_PROVEE"].ToString();
                txtFonoProveedorUno.Text = dr["TELEFONO_1_PROVEE"].ToString();
                txtFonoProveedor2.Text = dr["TELEFONO_2_PROVEE"].ToString();
                txtNombreServidor.Text = dr["NOM_SERVIDOR"].ToString();
                txtTelefonoServidor.Text = dr["TELEFONO_SERVIDOR"].ToString();

                btnRegistrar.IsEnabled = false;
                btnActualizar.IsEnabled = true;
                btnEliminar.IsEnabled = true;
                txtRutProveedor.IsEnabled = true;

            }
        }

        private async void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                if (ValidarRut(txtRutProveedor.Text))
                {
                    cmd.Parameters.Add("RUT_PROVEE", OracleDbType.Varchar2, 100).Value = txtRutProveedor.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "debe ingresar un rut valido!");
                    btnRegistrar.IsEnabled = true;
                    return;


                }

                if (txtNombreProveedor.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("NOMBRE_PROVEE", OracleDbType.Varchar2, 100).Value = txtNombreProveedor.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el nombre del/la proveedor(a) debe tener mas de 3 caracteres!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtDireccionProveedor.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("DIRECCION_PROVEE", OracleDbType.Varchar2, 100).Value = txtDireccionProveedor.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "La Dirección debe tener mas de 3 caracteres!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtFonoProveedorUno.Text.Replace(" ", string.Empty).Length == 9)
                {
                    cmd.Parameters.Add("TELEFONO_1_PROVEE", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtFonoProveedorUno.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Teléfono 1 del proveedor debe contener 9 dígitos!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtFonoProveedor2.Text.Replace(" ", string.Empty).Length == 9)
                {
                    cmd.Parameters.Add("TELEFONO_2_PROVEE", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtFonoProveedor2.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Teléfono 2 del proveedor debe contener 9 dígitos!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtNombreServidor.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("NOM_SERVIDOR", OracleDbType.Varchar2, 100).Value = txtNombreServidor.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Nombre del proveedor debe contener 3 o más caracteres!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }

                if (txtTelefonoServidor.Text.Replace(" ", string.Empty).Length == 9)
                {
                    cmd.Parameters.Add("TELEFONO_SERVIDOR", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtTelefonoServidor.Text);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "El Teléfono  del servidor debe contener 9 dígitos!");
                    btnRegistrar.IsEnabled = true;
                    return;

                }


                cmd.CommandText = "INSERT INTO PROVEEDOR(RUT_PROVEE,NOMBRE_PROVEE,DIRECCION_PROVEE,TELEFONO_1_PROVEE," +
               "TELEFONO_2_PROVEE,NOM_SERVIDOR ,TELEFONO_SERVIDOR) VALUES(:RUT_PROVEE,:NOMBRE_PROVEE,:DIRECCION_PROVEE," +
               ":TELEFONO_1_PROVEE,:TELEFONO_2_PROVEE,:NOM_SERVIDOR,:TELEFONO_SERVIDOR)";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("Agregado", "Proveedor se agregó correctamente");
                        this.resetAll();
                        this.ActualizarDataGrid();
                        btnRegistrar.IsEnabled = false;
                        btnActualizar.IsEnabled = true;
                        btnEliminar.IsEnabled = true;
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }



            }
            catch (Exception)
            {


            }
        }


        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
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

        private async void txtNombreProveedor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[a-zA-Z]"))
            {
                e.Handled = true;
                await this.ShowMessageAsync("Error", "Nombre del Proveedor debe contener sólo letras");
            }
            
        }

        private async void txtNombreServidor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[a-zA-Z]"))
            {
                e.Handled = true;
                await this.ShowMessageAsync("Error", "Nombre del seridor debe contener sólo letras");
            }
           
        }

        private async void txtFonoProveedorUno_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Teléfono1 del proveedor debe contener sólo números");
            }
        }

        private async void txtFonoProveedor2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Teléfono 2 del proveedor debe contener sólo números");
            }
        }

        private async void txtTelefonoServidor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Teléfono del servidor debe contener sólo números");
            }
        }
    }
}
