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
    /// Lógica de interacción para GestionarClientes.xaml
    /// </summary>
    public partial class GestionarClientes 
    {
        OracleConnection con = null;
        public GestionarClientes()
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

        private void ActualizarDataGrid()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT RUT_CLI , NOMBRE_CLI , APELLIDO_CLI , FONO_CLI , CORREO_CLI " +
                    "FROM CLIENTE ORDER BY NOMBRE_CLI ASC";
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dgClientes.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }

        }

        private void btnVolverAlMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
         
        }

 


        private async void btnModificar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("RUT_CLI", OracleDbType.Varchar2, 100).Value = txtRutCliente.Text;
                cmd.Parameters.Add("NOMBRE_CLI", OracleDbType.Varchar2, 100).Value = txtNombreCliente.Text;
                cmd.Parameters.Add("APELLIDO_CLI", OracleDbType.Varchar2, 100).Value = txtApellidoCliente.Text;
                cmd.Parameters.Add("FONO_CLI", OracleDbType.Int32, 20).Value = txtTelefonoCliente.Text;
                cmd.Parameters.Add("CORREO_CLI", OracleDbType.Varchar2, 100).Value = txtCorreoCliente.Text;

                cmd.CommandText = "update CLIENTE set rut_cli =:RUT_CLI,nombre_cli=:NOMBRE_CLI,apellido_cli=:APELLIDO_CLI,fono_cli=:FONO_CLI,correo_cli=:CORREO_CLI where rut_cli=:RUT_CLI";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("actualizado", "Cliente actualizado correctamente");
                        this.ActualizarDataGrid();

                    }
                }
                catch (Exception)
                {
                    await this.ShowMessageAsync("Error", "no se pudo actualizar");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void resetAll()
        {
            txtRutCliente.Text = "";
            txtNombreCliente.Text = "";
            txtApellidoCliente.Text = "";
            txtTelefonoCliente.Text = "";
            txtCorreoCliente.Text = "";


            btnAgregarCliente.IsEnabled = true;
            btnModificar.IsEnabled = false;
            btnEliminarCliente.IsEnabled = false;
        }

        private async void btnEliminarCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("RUT_CLI", OracleDbType.Varchar2, 100).Value = txtRutCliente.Text;
                cmd.CommandText = "delete from CLIENTE where rut_cli = :RUT_CLI";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("eliminado", "Cliente eliminado correctamente");
                        this.ActualizarDataGrid();
                        this.resetAll();

                    }
                }
                catch (Exception)
                {
                    await this.ShowMessageAsync("Error", "no se pudo eliminar");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            this.resetAll();

        }

        private void dgClientes_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActualizarDataGrid();
        }

        private void dgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                txtRutCliente.Text = dr["RUT_CLI"].ToString();
                txtNombreCliente.Text = dr["NOMBRE_CLI"].ToString();
                txtApellidoCliente.Text = dr["APELLIDO_CLI"].ToString();
                txtTelefonoCliente.Text = dr["FONO_CLI"].ToString();
                txtCorreoCliente.Text = dr["CORREO_CLI"].ToString();

                btnAgregarCliente.IsEnabled = false;
                btnModificar.IsEnabled = true;
                btnEliminarCliente.IsEnabled = true;

            }
        }

        private void btnVolverAlMenu_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private async void btnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.BindByName = true;

                cmd.Parameters.Add("RUT_CLI", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtRutCliente.Text);
                cmd.Parameters.Add("NOMBRE_CLI", OracleDbType.Varchar2, 100).Value = txtNombreCliente.Text;
                cmd.Parameters.Add("APELLIDO_CLI", OracleDbType.Varchar2, 100).Value = txtApellidoCliente.Text;
                cmd.Parameters.Add("FONO_CLI", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtTelefonoCliente.Text);
                cmd.Parameters.Add("CORREO_CLI", OracleDbType.Varchar2, 100).Value = txtCorreoCliente.Text;

                cmd.CommandText = "INSERT INTO CLIENTE(RUT_CLI , NOMBRE_CLI, APELLIDO_CLI , FONO_CLI , CORREO_CLI) " +
                                   "VALUES(:RUT_CLI,:NOMBRE_CLI ,:APELLIDO_CLI , :FONO_CLI , :CORREO_CLI)";
                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("Agregado", "Cliente se agregó correctamente");
                        this.ActualizarDataGrid();
                        resetAll();
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }


            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("Error", e.ToString());
            }
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnDeudas_Click(object sender, RoutedEventArgs e)
        {
            VisualizarDeudas vd = new VisualizarDeudas(txtRutCliente.Text);

            vd.Show();
            this.Close();
        }
    }
}
