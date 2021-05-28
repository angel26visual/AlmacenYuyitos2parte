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
using Microsoft.Win32;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para GestionarPromociones.xaml
    /// </summary>
    public partial class GestionarPromociones
    {
        OracleConnection con = null;
        public GestionarPromociones()
        {
            this.setConnection();
            InitializeComponent();
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

        private void cboTipoDeProducto_Loaded(object sender, RoutedEventArgs e)
        {
            this.CargarCboTipoDeProducto();
        }


        private void CargarCboTipoDeProducto()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_TIPPRODUC , DESCRIP_TIPPRODUC FROM TIPO_PRODUCTO";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboTipoDeProducto.ItemsSource = dt.AsDataView();
                cboTipoDeProducto.DisplayMemberPath = "DESCRIP_TIPPRODUC";
                cboTipoDeProducto.SelectedValuePath = "ID_TIPPRODUC";
            }
            catch (Exception)
            {

                throw;
            }
        } 

        private void CargarCboTipoDePromocion()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_TIPOPROMO , DESCRIP_TIPOPROMO FROM TIPO_PROMOCION";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboTipoDePromocion.ItemsSource = dt.AsDataView();
                cboTipoDePromocion.DisplayMemberPath = "DESCRIP_TIPOPROMO";
                cboTipoDePromocion.SelectedValuePath = "ID_TIPOPROMO";
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cboTipoDePromocion_Loaded(object sender, RoutedEventArgs e)
        {
            this.CargarCboTipoDePromocion();
        }

        private void btnVolverAlMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
    }
}

