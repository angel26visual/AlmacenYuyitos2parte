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
        public string nomUsusario { get; set; }
        DateTime date = DateTime.Today;
        public ConsultarPromociones()
        {
            this.setConnection();
            InitializeComponent();
            txtInicio.SelectedDate = DateTime.Today;
            txtTermino.SelectedDate = DateTime.Today;
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
            GestionarVentas gv = new GestionarVentas();
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
    }
}

