using MahApps.Metro.Controls.Dialogs;
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
    /// Lógica de interacción para ControlarRecepciones.xaml
    /// </summary>
    public partial class ControlarRecepciones
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        public ControlarRecepciones(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            nomUsuario = usuario;
            DatosUsuarios();
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

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
            this.Close();
        }

        private void btnIngresarRecepcion_Click(object sender, RoutedEventArgs e)
        {
            IngresarRecepcion ir = new IngresarRecepcion(nomUsuario);
            ir.Show();
            this.Close();
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

        private void dgRecepciones_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActualizarDataGrid();
        }

        private void ActualizarDataGrid()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_RECEPCION , VALOR_PAGADO_RECEP , FECH_RECEPCION , ORDEN_PED_ID_ORDEN FROM CONTROL_RECEP ORDER BY ID_RECEPCION ASC";
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dgRecepciones.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }

        }

        private async void btnVisualizarRecepcion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VerRecepcion vr = new VerRecepcion(nomUsuario);
                DataRowView dataRow = dgRecepciones.SelectedItem as DataRowView;
                if (dataRow != null)
                {
                    int recepcion = int.Parse(dataRow["ID_RECEPCION"].ToString());
                    vr.txtIdRecep.Text = dataRow["ID_RECEPCION"].ToString();
                    vr.txtValorAPagar.Text = dataRow["VALOR_PAGADO_RECEP"].ToString();
                    vr.dpFechaRecepcion.Text = dataRow["FECH_RECEPCION"].ToString();
                    vr.txtIdOrdenPedidoR.Text = dataRow["ORDEN_PED_ID_ORDEN"].ToString();
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = "SELECT CONTROL_RECEP_ID_RECEPCION AS ID_RECEPCION , PRODUCTO_COD_BARRA_PRODUCT AS COD_BARRA, NOM_PRODUCT AS NOMBRE_PRODUCTO, CANTIDAD_PRODUCT AS CANTIDAD, VALOR FROM DET_RECEP WHERE CONTROL_RECEP_ID_RECEPCION = :recepcion";
                    cmd.Parameters.Add("recepcion", OracleDbType.Int32, 20).Value = recepcion;
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader dr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(dr);
                    vr.dgDetalleR.ItemsSource = dt.DefaultView;
                    dr.Close();
                    vr.Show();
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
