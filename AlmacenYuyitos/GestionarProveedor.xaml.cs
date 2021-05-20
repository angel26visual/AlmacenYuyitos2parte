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
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para GestionarProveedor.xaml
    /// </summary>
    public partial class GestionarProveedor
    {
        OracleConnection con = null;
        public GestionarProveedor()
        {
            this.setConnection();
            InitializeComponent();
        }

        private void ActualizarDataGrid()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT RUT_PROVEE , NOMBRE_PROVEE , DIRECCION_PROVEE , TELEFONO_1_PROVEE , TELEFONO_2_PROVEE , NOM_SERVIDOR," +
                    "TELEFONO_SERVIDOR FROM PROVEEDOR ORDER BY NOMBRE_PROVEE ASC";
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dgProveedor.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }

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

        private void btnCerrarSesión_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnVolverAlMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }




        private void dgProveedor_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActualizarDataGrid();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            con.Close();
        }

       

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String sql = "DELETE FROM PROVEEDOR WHERE RUT_PROVEE = :RUT_PROVEE";
                this.AUD(sql, 2);
                this.resetAll();
            }
            catch (Exception)
            {

            }
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String sql = "UPDATE PROVEEDOR SET RUT_PROVEE = :RUT_PROVEE,NOMBRE_PROVEE = :NOMBRE_PROVEE,DIRECCION_PROVEE= :DIRECCION_PROVEE,TELEFONO_1_PROVEE= :TELEFONO_1_PROVEE," +
                "TELEFONO_2_PROVEE = :TELEFONO_2_PROVEE ,NOM_SERVIDOR = :NOM_SERVIDOR ,TELEFONO_SERVIDOR = :TELEFONO_SERVIDOR WHERE RUT_PROVEE = :RUT_PROVEE";
                this.AUD(sql, 1);
            }
            catch (Exception)
            {


            }
        }
        private void resetAll()
        {
            txtRutProveedor.Text = "";
            txtNombreProveedor.Text = "";
            txtDireccionProveedor.Text = "";
            txtFonoProveedorUno.Text = "";
            txtFonoProveedor2.Text = "";
            txtNombreServidor.Text = "";
            txtTelefonoServidor.Text = "";

            btnRegistrar.IsEnabled = true;
            btnActualizar.IsEnabled = false;
            btnEliminar.IsEnabled = false;
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            this.resetAll();
        }

        private void AUD(String sql_stm, int estado)
        {
            String msg = "";
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = sql_stm;
            cmd.CommandType = CommandType.Text;

            switch (estado)
            {
                case 0:
                    msg = "Proveedor Agregado Exitosamente";
                    cmd.Parameters.Add("RUT_PROVEE", OracleDbType.Varchar2, 100).Value = txtRutProveedor.Text;
                    cmd.Parameters.Add("NOMBRE_PROVEE", OracleDbType.Varchar2, 100).Value = txtNombreProveedor.Text;
                    cmd.Parameters.Add("DIRECCION_PROVEE", OracleDbType.Varchar2, 100).Value = txtDireccionProveedor.Text;
                    cmd.Parameters.Add("TELEFONO_1_PROVEE", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtFonoProveedorUno.Text);
                    cmd.Parameters.Add("TELEFONO_2_PROVEE", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtFonoProveedor2.Text);
                    cmd.Parameters.Add("NOM_SERVIDOR", OracleDbType.Varchar2, 100).Value = txtNombreServidor.Text;
                    cmd.Parameters.Add("TELEFONO_SERVIDOR", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtTelefonoServidor.Text);

                    break;
                case 1:
                    msg = "Proveedor Actualizado Exitosamente";
                    cmd.Parameters.Add("RUT_PROVEE", OracleDbType.Varchar2, 100).Value = txtRutProveedor.Text;
                    cmd.Parameters.Add("NOMBRE_PROVEE", OracleDbType.Varchar2, 100).Value = txtNombreProveedor.Text;
                    cmd.Parameters.Add("DIRECCION_PROVEE", OracleDbType.Varchar2, 100).Value = txtDireccionProveedor.Text;
                    cmd.Parameters.Add("TELEFONO_1_PROVEE", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtFonoProveedorUno.Text);
                    cmd.Parameters.Add("TELEFONO_2_PROVEE", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtFonoProveedor2.Text);
                    cmd.Parameters.Add("NOM_SERVIDOR", OracleDbType.Varchar2, 100).Value = txtNombreServidor.Text;
                    cmd.Parameters.Add("TELEFONO_SERVIDOR", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtTelefonoServidor.Text);
                    break;
                case 2:
                    msg = "Provedor Eliminado  Exitosamente";
                    cmd.Parameters.Add("RUT_PROVEE", OracleDbType.Varchar2, 100).Value = txtRutProveedor.Text;
                    break;

            }
            try
            {
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    MessageBox.Show(msg);
                    this.ActualizarDataGrid();
                }
            }
            catch (Exception expe)
            {

            }
        }

        private void dgProveedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                txtRutProveedor.Text = dr["RUT_PROVEE"].ToString();
                txtNombreProveedor.Text = dr["NOMBRE_PROVEE"].ToString();
                txtDireccionProveedor.Text = dr["DIRECCION_PROVEE"].ToString();
                txtFonoProveedorUno.Text = dr["TELEFONO_1_PROVEE"].ToString();
                txtFonoProveedor2.Text = dr["TELEFONO_2_PROVEE"].ToString();
                txtNombreServidor.Text = dr["NOM_SERVIDOR"].ToString();
                txtTelefonoServidor.Text = dr["TELEFONO_SERVIDOR"].ToString();

                btnRegistrar.IsEnabled = false;
                btnActualizar.IsEnabled = true;
                btnEliminar.IsEnabled = true;

            }
        }

        private void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String sql = "INSERT INTO PROVEEDOR(RUT_PROVEE,NOMBRE_PROVEE,DIRECCION_PROVEE,TELEFONO_1_PROVEE," +
                "TELEFONO_2_PROVEE,NOM_SERVIDOR ,TELEFONO_SERVIDOR) VALUES(:RUT_PROVEE,:NOMBRE_PROVEE,:DIRECCION_PROVEE," +
                ":TELEFONO_1_PROVEE,:TELEFONO_2_PROVEE,:NOM_SERVIDOR,:TELEFONO_SERVIDOR)";
                this.AUD(sql, 0);
                btnRegistrar.IsEnabled = false;
                btnActualizar.IsEnabled = true;
                btnEliminar.IsEnabled = true;
            }
            catch (Exception)
            {


            }
        }

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }
    }
}
