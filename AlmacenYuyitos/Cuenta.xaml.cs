using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Oracle.ManagedDataAccess.Client;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls.Dialogs;
using System.Data;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System.Drawing;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para Cuenta.xaml
    /// </summary>
    public partial class Cuenta
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        string rut = string.Empty;
        public Cuenta(string usuario)
        {
            nomUsuario = usuario;
            this.setConnection();
            InitializeComponent();
            actualizarEstadoC();
            String sql = "SELECT NOMBRE_TRAB, APELLIDO_TRAB, CORREO, NOM_USUARIO, ESTADO_CIVIL_ID_ESTAC, CARGO_TRABAJADOR_ID_CARGO FROM TRABAJADOR WHERE NOM_USUARIO = :USUARIO";
            this.AUD(sql, 0);
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

        private async void AUD(String sql_stmt, int state)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stmt;
            cmd.CommandType = CommandType.Text;
            try
            {
                switch (state)
                {
                    case 0:
                        string usuario = nomUsuario;
                        cmd.Parameters.Add("USUARIO", OracleDbType.Varchar2, 100).Value = usuario.ToString();
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            btnCuenta.Content = "Bienvenido/a " + reader["NOMBRE_TRAB"] + " " + reader["APELLIDO_TRAB"];
                            txtNombre.Text = reader["NOMBRE_TRAB"].ToString();
                            txtApellido.Text = reader["APELLIDO_TRAB"].ToString();
                            txtMail.Text = reader["CORREO"].ToString();
                            txtUsuario.Text = reader["NOM_USUARIO"].ToString();
                            cboEstadoCivil.SelectedValue = Convert.ToInt32(reader["ESTADO_CIVIL_ID_ESTAC"].ToString());
                            rut = reader["RUT_TRAB"].ToString();
                        }
                        else
                        {
                            await this.ShowMessageAsync("Información de contacto", "No se a podido traer la información del usuario");
                        }

                        break;
                    case 1:
                        cmd.Parameters.Add("CORREO", OracleDbType.Varchar2, 100).Value = txtMail.Text;
                        cmd.Parameters.Add("estadocivil", OracleDbType.Int32, 10).Value = cboEstadoCivil.SelectedValue;
                        cmd.Parameters.Add("RUT", OracleDbType.Varchar2, 9).Value = rut.ToString();
                        try
                        {
                            int n = cmd.ExecuteNonQuery();
                            if (n > 0)
                            {
                                await this.ShowMessageAsync("Información de contacto", "Se ha actualizado la información");

                            }
                        }
                        catch (Exception expe)
                        {
                            await this.ShowMessageAsync("Información de contacto", "No se a podido actualizar los registros");
                        }
                        break;
                    case 2:
                        cmd.Parameters.Add("USUARIO", OracleDbType.Varchar2, 100).Value = txtUsuario.Text;
                        cmd.Parameters.Add("contrasena", OracleDbType.Varchar2, 100).Value = txtNuevaContrasena.Password;
                        cmd.Parameters.Add("RUT", OracleDbType.Varchar2, 9).Value = rut.ToString();
                        try
                        {
                            int n = cmd.ExecuteNonQuery();
                            if (n > 0)
                            {
                                await this.ShowMessageAsync("Información de contacto", "Se ha actualizado la información");

                            }
                        }
                        catch (Exception expe)
                        {
                            await this.ShowMessageAsync("Información de contacto", "No se a podido actualizar los registros");
                        }
                        break;

                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }

        private async void actualizarEstadoC()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ID_ESTAC, DESCRIP_ESTAC FROM ESTADO_CIVIL";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable table = new DataTable();
                    adapter.Fill(table);
                    cboEstadoCivil.ItemsSource = table.AsDataView();
                    cboEstadoCivil.DisplayMemberPath = "DESCRIP_ESTAC";
                    cboEstadoCivil.SelectedValuePath = "ID_ESTAC";
                }
                else
                {
                    await this.ShowMessageAsync("CARGA ESTADO CIVIL", "No se han obtenidos información de los estados");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.nomUsuario = nomUsuario;
            main.Show();
            this.Close();
        }

        private void txtConfirmarPass_KeyUp(object sender, KeyEventArgs e)
        {
            if (txtNuevaContrasena.Password == " ")
            {
                txtNuevaContrasena.Password = "";
            }
            else if (txtConfirmarPass.Password == " ")
            {
                txtConfirmarPass.Password = "";
            }
            if (txtNuevaContrasena.Password.Trim() != "" || txtConfirmarPass.Password.Trim() != "")
            {
                if (txtConfirmarPass.Password.Trim() != txtNuevaContrasena.Password.Trim())
                {
                    lbContrasena.Content = "Las contrañas son diferentes";
                    lbContrasena.Foreground = new SolidColorBrush(Colors.Red);
                    lbContrasena.Visibility = Visibility.Visible;
                }
                else
                {
                    lbContrasena.Content = "Las contrañas son iguales";
                    lbContrasena.Foreground = new SolidColorBrush(Colors.Green);
                    lbContrasena.Visibility = Visibility.Visible;
                }
            }
            else
            {
                lbContrasena.Visibility = Visibility.Hidden;
            }

        }

        private void txtConfirmarPass_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int numKey = Convert.ToInt32(Convert.ToChar(e.Text));
            if (Convert.ToString(numKey) == "")
            {
                e.Handled = true;
                MessageBox.Show("funciono");
            }
            else
            {
                e.Handled = false;
            }
        }

        private void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            String sql = "UPDATE TRABAJADOR SET CORRE0 = :CORREO, ESTADO_CIVIL_ID_ESTAC = :ESTADOCIVIL WHERE RUT_TRAB = :RUT";
            this.AUD(sql, 1);
        }

        private void btnModificarUsuario_Click(object sender, RoutedEventArgs e)
        {
            String sql = "UPDATE TRABAJADOR SET NOM_USUARIO = :USUARIO, CONTRASENA_USUARIO = :CONTRASENA WHERE RUT_TRAB = :RUT";
            this.AUD(sql, 2);
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
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
    }
}
