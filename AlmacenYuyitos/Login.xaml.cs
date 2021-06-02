using MahApps.Metro.Controls.Dialogs;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System;
using System.Windows;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login
    {
        OracleConnection con = null;
        public Login()
        {
            this.setConnection();
            InitializeComponent();
            txtUser.Text = "juan.rojas";
            txtPass.Password = "juanjefeYuyito";
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

        private async void btnIniciar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                String sql = "SELECT CONTRASENA_USUARIO, NOMBRE_TRAB, APELLIDO_TRAB, NOM_USUARIO FROM TRABAJADOR WHERE NOM_USUARIO = :USUARIO";
                this.AUD(sql, 0);
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "No se pudo Iniciar Sesión ");
            }
        }

        private void btnLogin_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (LoginFlyout.IsOpen == true)
            {
                LoginFlyout.IsOpen = false;
            }
            else
            {
                LoginFlyout.IsOpen = true;
            }

        }

        private void btnEnviarContrasena_Click(object sender, RoutedEventArgs e)
        {
            String sql = "SELECT * FROM TRABAJADOR WHERE CORREO = :CORREO";
            this.AUD(sql, 1);
        }

        private void btnRecuperarPass_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (RecuperarContraseñaFlyouts.IsOpen == true)
            {
                RecuperarContraseñaFlyouts.IsOpen = false;
            }
            else
            {
                RecuperarContraseñaFlyouts.IsOpen = true;
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
                        cmd.Parameters.Add("USUARIO", OracleDbType.Varchar2, 40).Value = txtUser.Text;
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            if (txtPass.Password == reader["CONTRASENA_USUARIO"].ToString())
                            {
                                MainWindow main = new MainWindow();
                                main.Show();
                                main.btnCuenta.Content = "Bienvenido/a " + reader["NOMBRE_TRAB"] + " " + reader["APELLIDO_TRAB"];
                                main.nomUsuario = reader["NOM_USUARIO"].ToString();
                                this.Close();
                            }
                            else
                            {
                                await this.ShowMessageAsync("INICIO DE SESIÓN", "Contraseña incorrecta");
                            }
                        }
                        else
                        {
                            await this.ShowMessageAsync("INICIO DE SESIÓN", "El usuario no esta registrado");
                        }
                        break;
                    case 1:
                        cmd.Parameters.Add("CORREO", OracleDbType.Varchar2, 100).Value = txtCorreo.Text;
                        OracleDataReader reader2 = cmd.ExecuteReader();
                        if (reader2.Read())
                        {
                            generarNuevaContrasena(txtCorreo.Text);
                        }
                        else
                        {
                            await this.ShowMessageAsync("RECUPERAR CONTRASEÑA", "Correo incorrecto");
                        }
                        break;

                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }

        public async void generarNuevaContrasena(string email)
        {
            Random rd = new Random(DateTime.Now.Millisecond);
            int nuevaContrasena = rd.Next(1000000, 999999999);
            string contrasena = "" + nuevaContrasena;
            OracleCommand cmd = con.CreateCommand();
            String sql_mt = "UPDATE TRABAJADOR SET CONTRASENA_USUARIO = :CONTRASENA WHERE CORREO = :CORREO";
            cmd.CommandText = sql_mt;
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("CONTRASENA", OracleDbType.Varchar2, 50).Value = contrasena.ToString();
            cmd.Parameters.Add("CORREO", OracleDbType.Varchar2, 100).Value = email.ToString();
            try
            {
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    enviarContrasena(nuevaContrasena, email);
                }
                else
                {
                    await this.ShowMessageAsync("Error", "No se actualizo");
                }
            }
            catch (Exception exp)
            {
                await this.ShowMessageAsync("Error", "No se envío nueva contraseña" + exp.Message);
            }

        }

        public void enviarContrasena(int contrasena, string correo)
        {
            string mensaje = string.Empty;
            //Creando el correo electronico
            string destinatario = correo;
            string remitente = "juanito.rojasyuyitos@gmail.com";
            string asunto = "Nueva contraseña Almacen 'Los Yuyitos'";
            string cuerpoDelMensaje = "Su nueva contraseña es: " + Convert.ToString(contrasena) + " se recomienda cambiar la contraseña la proxima vez que ingrese al sistema";
            MailMessage ms = new MailMessage(remitente, destinatario, asunto, cuerpoDelMensaje);

            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.EnableSsl = true;
            smtp.Credentials = new NetworkCredential("juanito.rojasYuyitos@gmail.com", "juanjefeYuyito");
            try
            {
                Task.Run(() =>
                {
                    smtp.Send(ms);
                    ms.Dispose();
                    MessageBox.Show("Contraseña enviada, revise su correo");
                }
                );
                MessageBox.Show("Puede tardar unos segundos, por favor espere");


            }
            catch (Exception)
            {
                MessageBox.Show("Error, No se ha podido enviar la contraseña");
            }
        }
    }
}

