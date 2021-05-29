using Oracle.ManagedDataAccess.Client;
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
    /// Lógica de interacción para VisualizarVentas.xaml
    /// </summary>
    public partial class VisualizarVentas 
    {
        OracleConnection con = null;
        string commandtable = "SELECT NRO_BOLETA, FECHA_VENTA, TOTAL_VENTA, TOTAL_DESCUENTOS, MONTO_TOTAL, MONTO_PAGO, TRABAJADOR_RUT_TRAB FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 1"; 
        public VisualizarVentas()
        {
            setConnection();
            InitializeComponent();
            actualizarDataGrid();
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
        private void actualizarDataGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = commandtable;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dgVenta.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarVentas gv = new GestionarVentas();
            gv.Show();
            this.Close();
        }

        private void btnVerVentas_Click(object sender, RoutedEventArgs e)
        {
            VerVenta verv = new VerVenta();
            verv.Show();
            this.Close();
        }

        private void btnVer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
