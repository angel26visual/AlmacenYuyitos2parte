
using System.Data;
using MahApps.Metro.Controls.Dialogs;

using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para AgregarUsuario.xaml
    /// </summary>
    public partial class AgregarUsuario
    {
        OracleConnection con = null;
        public AgregarUsuario()
        {
            this.setConnection();
            InitializeComponent();
            ActualizarCargo();
            ActualizarEstado();
        }

        private void btnVolver_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GestionarUsuarios gu = new GestionarUsuarios();
            gu.Show();
            this.Close();
        }

        private void btnCerrarSesion_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
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

        private void cboEstadoCivil_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {


            ActualizarEstado();

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

        private void cboCargo_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ActualizarCargo();

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


        private async void btmGuardar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                if (ValidarRut(txtRut.Text))
                { cmd.Parameters.Add("RUT_TRAB", OracleDbType.Varchar2, 100).Value = txtRut.Text; }
                else { await this.ShowMessageAsync("Error", "debe ingresar un rut valido!"); }
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
                if (cboEstadoCivil.SelectedValue != null)
                { cmd.Parameters.Add("ESTADO_CIVIL_ID_ESTAC", OracleDbType.Int32, 20).Value = cboEstadoCivil.SelectedValue; }
                else { await this.ShowMessageAsync("Error", "debe seleccionar un estado civil!"); }
                if (cboCargo.SelectedValue != null)
                { cmd.Parameters.Add("CARGO_TRABAJADOR_ID_CARGO", OracleDbType.Int32, 20).Value = cboCargo.SelectedValue; }
                else { await this.ShowMessageAsync("Error", "debe seleccionar un cargo!"); }

                cmd.CommandText = "INSERT INTO TRABAJADOR(RUT_TRAB,NOMBRE_TRAB,APELLIDO_TRAB,FECHA_NACIMIENTO," +
                "CORREO,NOM_USUARIO ,CONTRASENA_USUARIO,CARGO_TRABAJADOR_ID_CARGO, ESTADO_CIVIL_ID_ESTAC) VALUES(:RUT_TRAB,:NOMBRE_TRAB,:APELLIDO_TRAB," +
                ":FECHA_NACIMIENTO,:CORREO,:NOM_USUARIO,:CONTRASENA_USUARIO,:CARGO_TRABAJADOR_ID_CARGO,:ESTADO_CIVIL_ID_ESTAC)";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("Agregado", "Usuario se agregó correctamente");
                        Limpiar();
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }


            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("Error", ex.ToString());

            }
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
        private void Limpiar()
        {
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtRut.Text = string.Empty;
            dpFechaNacimiento.SelectedDate = null;
            txtCorreo.Text = string.Empty;
            cboEstadoCivil.Text = null;
            cboCargo.SelectedValue = null;
            txtNombreUsuario.Text = string.Empty;
            txtContrasena.Password = string.Empty;
        }

    }
}




