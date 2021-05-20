using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using System;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para AgregarUsuario.xaml
    /// </summary>
    public partial class AgregarUsuario
    {
        OracleConnection con = null;
        public AgregarUsuario()
        {
            this.setConnection();
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GestionarUsuarios gu = new GestionarUsuarios();
            gu.Show();
            this.Close();
        }

        private void btnCerrarSesion_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
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

        private void cboEstadoCivil_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {


            ActualizarEstado();
            
        }

        private void ActualizarEstado()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_ESTAC , DESCRIP_ESTAC FROM ESTADO_CIVIL";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                 
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboEstadoCivil.ItemsSource = dt.AsDataView();
                cboEstadoCivil.DisplayMemberPath = "DESCRIP_ESTAC";
                cboEstadoCivil.SelectedValuePath = "ID_ESTAC";

            }
            catch (Exception e)
            {
            }

        }

        private void cboCargo_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ActualizarCargo();

        }

        private void ActualizarCargo()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_CARGO , NOMBRE_CARGO FROM CARGO_TRABAJADOR";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboCargo.ItemsSource = dt.AsDataView();
                cboCargo.DisplayMemberPath = "NOMBRE_CARGO";
                cboCargo.SelectedValuePath = "ID_CARGO";

            }
            catch (Exception e)
            {
            }

        }
    }
    }


