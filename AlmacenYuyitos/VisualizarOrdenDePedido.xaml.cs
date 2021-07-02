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
using BibliotecaLosYuyitos;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para VisualizarOrdenDePedido.xaml
    /// </summary>
    public partial class VisualizarOrdenDePedido 
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
      
       
        public VisualizarOrdenDePedido(string usuario)
        {
         
            this.setConnection();
            InitializeComponent();
            nomUsuario = usuario;

            DatosUsuarios();
            ActualizarDataGrid();
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
                cmd.CommandText = "SELECT ID_ORDEN , FECH_ORDEN , MONTO_ORDEN , PROVEEDOR_RUT_PROVEE , DESCRIP_ORDEN FROM ORDEN_PED ORDER BY ID_ORDEN ASC";
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dgOrdenes.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }

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

        private void dgOrdenes_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActualizarDataGrid();
        }

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            MenuOrdenDePedidos mop = new MenuOrdenDePedidos(nomUsuario);
            mop.Show();
            this.Close();
        }

        private async void btnVisualizarOrden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VerOrden orden = new VerOrden(nomUsuario);
                DataRowView dataRow = dgOrdenes.SelectedItem as DataRowView;
                if (dataRow != null)
                {
                    orden.ID_orden = int.Parse(dataRow["ID_ORDEN"].ToString());
                    orden.txtIdOrdenPedidos.Text = dataRow["ID_ORDEN"].ToString();
                    orden.dpFechaOrdenPedido.Text = dataRow["FECH_ORDEN"].ToString();
                    orden.txtMontoTotal.Text = dataRow["MONTO_ORDEN"].ToString();
                    orden.cboProveedor.Text = dataRow["PROVEEDOR_RUT_PROVEE"].ToString();
                    orden.txtDescripcion.Text = dataRow["DESCRIP_ORDEN"].ToString();
                    orden.Show();
                    this.Close();
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }
    }
}
