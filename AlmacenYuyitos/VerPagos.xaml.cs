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
using MahApps.Metro.Controls.Dialogs;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para verDeudas.xaml
    /// </summary>
    public partial class verPagos
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        string script = "select p.id_pago, b.cliente_rut_cli rut_cliente , p.fech_pago, p.monto_pago, m.descrip_mediopago,p.boleta_nro_boleta from pago p join medio_pago m on m.id_mediopago= p.medio_pago_id_mediopago join boleta b on b.nro_boleta=p.boleta_nro_boleta";

        public verPagos(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            cargarPagos();
            nomUsuario = usuario;
            DatosUsuarios();
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
        private void cargarPagos()
        {
            OracleCommand cmd = con.CreateCommand();

            cmd.CommandText = script;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dgVerPagos.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private void btnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = con.CreateCommand();
            try
            {
                cmd.CommandText = script + " where b.cliente_rut_cli= :CLIENTE ";
                cmd.Parameters.Add("CLIENTE", OracleDbType.Varchar2, 100).Value = txtRut.Text;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dgVerPagos.ItemsSource = dt.DefaultView;
                dr.Close();
            }
            catch (Exception)
            {
                dgVerPagos.ItemsSource = null;
                dgVerPagos.Items.Refresh();

            }

        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            cargarPagos();
        }

        private void btnvolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarPagos gestionar = new GestionarPagos(nomUsuario);
            gestionar.Show();
            this.Close();
        }

        private void btnCuenta_Click(object sender, System.Windows.RoutedEventArgs e)
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

        private async void btnCerrarSesion_Click(object sender, System.Windows.RoutedEventArgs e)
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
