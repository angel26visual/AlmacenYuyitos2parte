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
using Oracle.ManagedDataAccess.Types;



namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para GenerarOrdenDePedidos.xaml
    /// </summary>
    public partial class GenerarOrdenDePedidos 
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        
        public GenerarOrdenDePedidos(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            ActualizarDataGrid();
            nomUsuario = usuario;
            DatosUsuarios();
            CargaCboTipoDeProducto();
            dpFechaOrdenPedido.Text = Convert.ToString(DateTime.Today);
            
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

      

        private async void txtIdOrdenPedidos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El ID de la Orden de pedidos debe contener sólo números");
            }
        }

      

        private async void txtMontoTotal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Monto Total debe contener sólo números");
            }
        }

        private void btnLimpiarCampos_Click(object sender, RoutedEventArgs e)
        {
            this.resetAll();
        }

        private async void btnGuardarOrdenPedido_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_insertar_orden",con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("id", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdOrdenPedidos.Text);

                if (dpFechaOrdenPedido.SelectedDate != null)
                { cmd.Parameters.Add("fecha", OracleDbType.Date).Value = dpFechaOrdenPedido.SelectedDate; }

                else { 
                    await this.ShowMessageAsync("Error", "la fecha de orden no puede estar nula");
                    return;
                }
                cmd.Parameters.Add("monto", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtMontoTotal.Text);
                cmd.Parameters.Add("proveedor", OracleDbType.Varchar2, 100).Value = cboProveedor.Text;

                if (txtProductos.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("descripcion", OracleDbType.Varchar2, 100).Value = txtProductos.Text;

                }
                else
                {
                    await this.ShowMessageAsync("Error", "La descripción de la Orden debe tener mas de 3 caracteres!");
                    btnGuardarOrdenPedido.IsEnabled = true;
                    return;

                }

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n < 0)
                    {
                        await this.ShowMessageAsync("Agregada", "Orden de Pedido se agregó correctamente");
                        this.ActualizarDataGrid();
                        resetAll();
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void btnModificarOrden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_actualizar_orden", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("id", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdOrdenPedidos.Text);

                if (dpFechaOrdenPedido.SelectedDate != null)
                { cmd.Parameters.Add("fecha", OracleDbType.Date).Value = dpFechaOrdenPedido.SelectedDate; }

                else
                {
                    await this.ShowMessageAsync("Error", "la fecha de orden no puede estar nula");
                    return;
                }
                cmd.Parameters.Add("monto", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtMontoTotal.Text);
                cmd.Parameters.Add("rut_proveedor", OracleDbType.Varchar2, 100).Value = cboProveedor.Text;

                if (txtProductos.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("descripcion", OracleDbType.Varchar2, 100).Value = txtProductos.Text;

                }
                else
                {
                    await this.ShowMessageAsync("Error", "La descripción de la Orden debe tener mas de 3 caracteres!");
                    btnGuardarOrdenPedido.IsEnabled = true;
                    return;

                }
                    int n = cmd.ExecuteNonQuery();
                    if (n < 0)
                    {
                        await this.ShowMessageAsync("actualizada", "Orden Actualizada Exitosamente");
                        this.ActualizarDataGrid();
                        this.resetAll();

                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "no se pudo actualizar");
                    }
                
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void btnEliminarOrden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_eliminar_orden", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("id_orden", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdOrdenPedidos.Text);

                MessageDialogResult respuesta = await this.ShowMessageAsync("ELIMINAR", "¿Desea Eliminar Información de la orden Seleccionada?", MessageDialogStyle.AffirmativeAndNegative);

                if (respuesta == MessageDialogResult.Affirmative)
                {
                    int n = cmd.ExecuteNonQuery();

                    if (n < 0)
                    {
                        await this.ShowMessageAsync("eliminada", "Orden de Pedido Eliminada correctamente");
                        this.ActualizarDataGrid();
                        this.resetAll();

                    }

                }

                else
                {
                    return;
                }


               
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("Error", ex.ToString());
            }
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
                dgOrdenPedido.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }

        }

        private void ActualizarDataGridProveedor()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT RUT_PROVEE , NOMBRE_PROVEE , NOM_SERVIDOR , TELEFONO_SERVIDOR FROM PROVEEDOR ORDER BY NOMBRE_PROVEE ASC";
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

        private void dgOrdenPedido_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActualizarDataGrid();
        }

        private void resetAll()
        {
            txtIdOrdenPedidos.Text = "";
            txtMontoTotal.Text = "";
            txtProductos.Text = "";
           

            btnGuardarOrdenPedido.IsEnabled = true;
            btnModificarOrden.IsEnabled = false;
            btnEliminarOrden.IsEnabled = false;
            txtIdOrdenPedidos.IsEnabled = true;
            dpFechaOrdenPedido.IsEnabled = true;

        }

        private void dgOrdenPedido_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                txtIdOrdenPedidos.Text = dr["ID_ORDEN"].ToString();
                dpFechaOrdenPedido.Text = dr["FECH_ORDEN"].ToString();
                txtMontoTotal.Text = dr["MONTO_ORDEN"].ToString();
                cboProveedor.Text = dr["PROVEEDOR_RUT_PROVEE"].ToString();
                txtProductos.Text = dr["DESCRIP_ORDEN"].ToString();


                btnGuardarOrdenPedido.IsEnabled = false;
                btnModificarOrden.IsEnabled = true;
                btnEliminarOrden.IsEnabled = true;
                txtIdOrdenPedidos.IsEnabled = false;
                dpFechaOrdenPedido.IsEnabled = false;

            }
        }

        private void CargaCboTipoDeProducto()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NOM_SERVIDOR , RUT_PROVEE FROM PROVEEDOR";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboProveedor.ItemsSource = dt.AsDataView();
                cboProveedor.DisplayMemberPath = "RUT_PROVEE";
                cboProveedor.SelectedValuePath = "NOM_SERVIDOR";
            }
            catch (Exception)
            {


            }
        }

        private void dgProveedor_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActualizarDataGridProveedor();
        }

        private void dgProveedor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
               
                cboProveedor.Text = dr["RUT_PROVEE"].ToString();
                


                btnGuardarOrdenPedido.IsEnabled = false;
                btnModificarOrden.IsEnabled = true;
                btnEliminarOrden.IsEnabled = true;
                txtIdOrdenPedidos.IsEnabled = true;
                dpFechaOrdenPedido.IsEnabled = true; ;

            }
        }
    }
}
