using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Lógica de interacción para RegistrarPago.xaml
    /// </summary>
    public partial class RegistrarPago 
    {
        OracleConnection con = null;
        public string nomUsusario { get; set; }
        public RegistrarPago()
        {
            this.setConnection();
            InitializeComponent();
            txtFechaVenta.SelectedDate = DateTime.Today;
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

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarVentas gv = new GestionarVentas();
            gv.Show();
            this.Close();
        }

        private void btnRealizarPago_Click(object sender, RoutedEventArgs e)
        {
            RealizarPagoVenta rpv = new RealizarPagoVenta();
            rpv.Show();
            this.Close();
        }
    }
}
