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
    /// Lógica de interacción para ConsultarPromociones.xaml
    /// </summary>
    public partial class ConsultarPromociones
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        DateTime date = DateTime.Today;
        public ConsultarPromociones(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            nomUsuario = usuario;
            txtInicio.SelectedDate = DateTime.Today;
            txtTermino.SelectedDate = DateTime.Today;
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

        private async void btnFiltrarInicio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String sql = "SELECT ID_PROMOCION, IMAGEN_PROMO, FECHA_INICIO_PROMO, FECHA_FIN_PROMO, DESCRIP_PROMO FROM PROMOCION WHERE FECHA_INICIO_PROMO >= :FECHA_INICIO ORDER BY FECHA_INICIO_PROMO ASC";
                this.AUD(sql, 0);
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void btnFiltrarTermino_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String sql = "SELECT ID_PROMOCION, IMAGEN_PROMO, FECHA_INICIO_PROMO, FECHA_FIN_PROMO, DESCRIP_PROMO FROM PROMOCION WHERE FECHA_INICIO_PROMO >= :FECHA_INICIO AND FECHA_FIN_PROMO <= :FECHA_TERMINO ORDER BY FECHA_INICIO_PROMO ASC";
                this.AUD(sql, 1);
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void btnFiltrarFechas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String sql = "SELECT ID_PROMOCION, IMAGEN_PROMO, FECHA_INICIO_PROMO, FECHA_FIN_PROMO, DESCRIP_PROMO FROM PROMOCION WHERE FECHA_INICIO_PROMO >= :FECHA_INICIO AND FECHA_FIN_PROMO <= :FECHA_TERMINO ORDER BY FECHA_INICIO_PROMO ASC";
                this.AUD(sql, 2);
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
                    cmd.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = txtInicio.SelectedDate;
                    try
                    {
                        OracleDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgPromociones.ItemsSource = dt.DefaultView;
                            reader.Close();
                        }
                        else
                        {
                            dgPromociones.ItemsSource = null;
                            await this.ShowMessageAsync("Promociones", "No hay promociones con fecha de inicio mayor a la ingresada");
                        }
                    }
                    catch (Exception )
                    {
                        await this.ShowMessageAsync("Error", "Ha ocurrido un error");
                    }
                    break;
                case 1:
                    cmd.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = date;
                    cmd.Parameters.Add("FECHA_TERMINO", OracleDbType.Date).Value = txtTermino.SelectedDate;
                    try
                    {
                        OracleDataReader reader2 = cmd.ExecuteReader();
                        if (reader2.Read())
                        {
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgPromociones.ItemsSource = dt.DefaultView;
                            reader2.Close();
                        }
                        else
                        {
                            dgPromociones.ItemsSource = null;
                            await this.ShowMessageAsync("Promociones", "No hay promociones desde la fecha actual con fecha de termino menor a la ingresada");
                        }
                    }
                    catch (Exception e)
                    {
                        await this.ShowMessageAsync("Error", "Ha ocurrido un error");
                    }

                    break;
                case 2:
                    cmd.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = txtInicio.SelectedDate;
                    cmd.Parameters.Add("FECHA_TERMINO", OracleDbType.Date).Value = txtTermino.SelectedDate;
                    try
                    {
                        OracleDataReader reader3 = cmd.ExecuteReader();
                        if (reader3.Read())
                        {
                            OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            dgPromociones.ItemsSource = dt.DefaultView;
                            reader3.Close();
                        }
                        else
                        {
                            dgPromociones.ItemsSource = null;
                            await this.ShowMessageAsync("Promociones", "No hay promociones entre las fechas ingresadas");
                        }
                    }
                    catch (Exception e)
                    {
                        await this.ShowMessageAsync("Error", "Ha ocurrido un error");
                    }

                    break;

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
    }
}

