using System;
using System.Configuration;
using System.Data;
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
using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;


namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para ModyElmUsuario.xaml
    /// </summary>
    public partial class ModyElmUsuario
    {
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        OracleConnection con = null;
        public ModyElmUsuario(string usuario)
        {
            setConnection();
            InitializeComponent();
            nomUsuario = usuario;
            ActualizarCargo();
            ActualizarEstado();
            DatosUsuarios();
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

        private void ActualizarCargo()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_CARGO , NOMBRE_CARGO FROM CARGO_TRABAJADOR";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboCargo.ItemsSource = dt.AsDataView();
                cboCargo.DisplayMemberPath = "NOMBRE_CARGO";
                cboCargo.SelectedValuePath = "ID_CARGO";

            }
            catch (Exception e)
            {
            }

        }
        private void ActualizarEstado()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_ESTAC , DESCRIP_ESTAC FROM ESTADO_CIVIL";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboEstadoCivil.ItemsSource = dt.AsDataView();
                cboEstadoCivil.DisplayMemberPath = "DESCRIP_ESTAC";
                cboEstadoCivil.SelectedValuePath = "ID_ESTAC";

            }
            catch (Exception e)
            {
            }

        }
        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            VisualizarUsuario vu = new VisualizarUsuario(nomUsuario);
            vu.Show();
            this.Close();
        }


        private async void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                if (txtNombre.Text.Replace(" ", string.Empty).Length >= 3)
                { cmd.Parameters.Add("NOMBRE_TRAB", OracleDbType.Varchar2, 100).Value = txtNombre.Text; }
                else { await this.ShowMessageAsync("Error", "el nombre debe tener mas de 3 caracteres!"); }
                if (txtApellido.Text.Replace(" ", string.Empty).Length >= 3)
                { cmd.Parameters.Add("APELLIDO_TRAB", OracleDbType.Varchar2, 100).Value = txtApellido.Text; }
                else { await this.ShowMessageAsync("Error", "el apellido debe tener mas de 3 caracteres!"); }
                if (dpFechaNacimiento.SelectedDate != null)
                { cmd.Parameters.Add("FECHA_NACIMIENTO", OracleDbType.Date).Value = dpFechaNacimiento.SelectedDate; }
                else { await this.ShowMessageAsync("Error", "la fecha de nacimiento debe ser valida!"); }
                if (txtCorreo.Text.Replace(" ", string.Empty) != null)
                { cmd.Parameters.Add("CORREO", OracleDbType.Varchar2, 100).Value = txtCorreo.Text; }
                else { await this.ShowMessageAsync("Error", "debe ingresar un correo!"); }
                if (txtNombreUsuario.Text.Replace(" ", string.Empty).Length >= 3)
                { cmd.Parameters.Add("NOM_USUARIO", OracleDbType.Varchar2, 100).Value = txtNombreUsuario.Text; }
                else { await this.ShowMessageAsync("Error", "el usuario debe tener mas de 3 caracteres!"); }
                if (txtContrasena.Password.Replace(" ", string.Empty).Length >= 6)
                { cmd.Parameters.Add("CONTRASENA_USUARIO", OracleDbType.Varchar2, 100).Value = txtContrasena.Password; }
                else { await this.ShowMessageAsync("Error", "la contraseña debe tener mas de 6 caracteres!"); }
                if (cboCargo.SelectedValue != null)
                { cmd.Parameters.Add("CARGO_TRABAJADOR_ID_CARGO", OracleDbType.Int32, 20).Value = cboCargo.SelectedValue; }
                else { await this.ShowMessageAsync("Error", "debe seleccionar un cargo!"); }
                if (cboEstadoCivil.SelectedValue != null)
                { cmd.Parameters.Add("ESTADO_CIVIL_ID_ESTAC", OracleDbType.Int32, 20).Value = cboEstadoCivil.SelectedValue; }
                else { await this.ShowMessageAsync("Error", "debe seleccionar un estado civil!"); }
                cmd.Parameters.Add("RUT_TRAB", OracleDbType.Varchar2, 100).Value = txtRut.Text;
                cmd.CommandText = "update trabajador set nombre_trab =:NOMBRE_TRAB,apellido_trab=:APELLIDO_TRAB,fecha_nacimiento=:FECHA_NACIMIENTO,correo=:CORREO,nom_usuario=:NOM_USUARIO,contrasena_usuario=:CONTRASENA_USUARIO,cargo_trabajador_id_cargo=:CARGO_TRABAJADOR_ID_CARGO,estado_civil_id_estac=:ESTADO_CIVIL_ID_ESTAC where rut_trab=:RUT_TRAB";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("actualizado", "Usuario actualizado correctamente");
                        VisualizarUsuario v = new VisualizarUsuario(nomUsuario);
                        this.Close();
                        v.Show();

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

        private async void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("RUT_TRAB", OracleDbType.Varchar2, 100).Value = txtRut.Text;
                cmd.CommandText = "delete from trabajador where rut_trab = :RUT_TRAB";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("eliminado", "Usuario eliminado correctamente");
                        VisualizarUsuario v = new VisualizarUsuario(nomUsuario);
                        this.Close();
                        v.Show();

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

