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
    /// Lógica de interacción para VisualizarDeudas.xaml
    /// </summary>
    public partial class VisualizarDeudas
    {
        OracleConnection con = null;

        public VisualizarDeudas(string rut)
        {
            InitializeComponent();
            setConnection();
            ActualizarDataGrid(rut);
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

        private void ActualizarDataGrid(string rut)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.Parameters.Add("RUT", OracleDbType.Varchar2, 100).Value = rut;
                cmd.CommandText = "SELECT id_deuda id, fech_deuda fecha , cliente_rut_cli rut, monto_deuda monto,e.descripcion_estadeuda estado , estado_deuda_id_estadeuda estado_id from deuda d join estado_deuda e on d.estado_deuda_id_estadeuda=e.id_estadeuda where cliente_rut_cli = :RUT";

                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dtgDeudas.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }

        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarClientes myec = new GestionarClientes();
            myec.Show();
            this.Close();
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

        private void btnVerInformacion_Click(object sender, RoutedEventArgs e)
        {
            VerInformacion vi = new VerInformacion();
            DataRowView datos = dtgDeudas.SelectedItem as DataRowView;
            if (datos != null)
            {
                vi.txtRutlienteDeuda.Text = datos["RUT"].ToString();
                vi.txtDeudaId.Text = datos["ID"].ToString();
                vi.dpFechaDeuda.Text = datos["FECHA"].ToString();
                vi.txtMontoDeuda.Text = datos["MONTO"].ToString();
                vi.cboEstado.SelectedValue = int.Parse(datos["ESTADO_ID"].ToString());


                vi.Show();
                this.Close();
            }
        }
    }
}
