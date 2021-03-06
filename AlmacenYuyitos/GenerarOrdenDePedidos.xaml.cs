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
using BibliotecaLosYuyitos;



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
        DateTime fecha = DateTime.Today;
        List<Detalle_Orden> listOrden = new List<Detalle_Orden>();
        int montoTotal = 0;

        public GenerarOrdenDePedidos(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            ActualizarDataGrid();
            nomUsuario = usuario;
            DatosUsuarios();
            CargaCboProveedor();
            resetAll();
            
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
            MenuOrdenDePedidos mop = new MenuOrdenDePedidos(nomUsuario);
            mop.Show();
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
                OracleCommand cmd = new OracleCommand("sp_insertar_orden", con);
                cmd.CommandType = CommandType.StoredProcedure;

                bool registrado = existeOrden(int.Parse(txtIdOrdenPedidos.Text));
                if (registrado)
                {
                    await this.ShowMessageAsync("Error", "Orden ya se encuentra registrada");
                }
                else
                {
                    if (listOrden.Count() > 0)
                    {
                        cmd.Parameters.Add("id", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdOrdenPedidos.Text);

                        if (dpFechaOrdenPedido.SelectedDate != null)
                        { cmd.Parameters.Add("fecha", OracleDbType.Date).Value = dpFechaOrdenPedido.SelectedDate; }

                        else
                        {
                            await this.ShowMessageAsync("Error", "la fecha de orden no puede estar nula");
                            return;
                        }
                        cmd.Parameters.Add("monto", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtMontoTotal.Text);
                        cmd.Parameters.Add("proveedor", OracleDbType.Varchar2, 100).Value = cboProveedor.Text;

                        if (txtDescripcion.Text.Replace(" ", string.Empty).Length >= 3)
                        {
                            cmd.Parameters.Add("descripcion", OracleDbType.Varchar2, 100).Value = txtDescripcion.Text;

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
                            foreach (var detalleO in listOrden)
                            {
                                GuardarDetalle(detalleO.Cantidad, detalleO.Id_orden, detalleO.Nombre_producto, detalleO.Codigo_barra, detalleO.Valor);
                            }
                            await this.ShowMessageAsync("Agregada", "Orden de Pedido se agregó correctamente");
                            resetAll();
                        }
                        else
                        {
                            await this.ShowMessageAsync("Agregada", "Orden de Pedido no se agregó correctamente");
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("ORDEN DE PEDIDO", "No hay productos agregado a la lista");
                    }
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private bool existeOrden(int orden)
        {
            bool respuesta = false;
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "select * from orden_ped where ID_ORDEN =:orden";
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add("id", OracleDbType.Int32, 20).Value = orden;
            OracleDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                respuesta = true;
            }
            return respuesta;
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
                //dgOrdenPedido.ItemsSource = dt.DefaultView;
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
            GenerarIdOrden();
            txtMontoTotal.Text = 0.ToString();
            txtDescripcion.Text = "";
            cboProveedor.SelectedValue = 0;
            dpFechaOrdenPedido.Text = Convert.ToString(DateTime.Today);
            txtCantidad.Text = 0.ToString();
            txtValor.Text = 0.ToString();
            txtCodigoBarra.Text = 0.ToString();
            txtNombreProducto.Text = "";
            for (int i = listOrden.Count -1; i >= 0; i--)
            {
                listOrden.RemoveAt(i);
            }
            dgDetalleOrden.ItemsSource = null;
            dgDetalleOrden.ItemsSource = listOrden;
            montoTotal = 0;

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
                txtDescripcion.Text = dr["DESCRIP_ORDEN"].ToString();


                btnGuardarOrdenPedido.IsEnabled = false;
                //btnModificarOrden.IsEnabled = true;
                //btnEliminarOrden.IsEnabled = true;
                txtIdOrdenPedidos.IsEnabled = false;
                dpFechaOrdenPedido.IsEnabled = false;

            }
        }

        private async void CargaCboProveedor()
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
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
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

            }
        }

        private async void GenerarIdOrden()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(MAX(ID_ORDEN), 8000) AS ORDEN FROM ORDEN_PED";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int nro_orden = int.Parse(reader["ORDEN"].ToString());
                    nro_orden = nro_orden + 1;
                    txtIdOrdenPedidos.Text = nro_orden.ToString();
                }
                else
                {
                    await this.ShowMessageAsync("ORDEN DE PEDIDO", "No se ha podido obtener el número de la orden");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void btnAgregarP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int total = 0;
                int productoEnlista = 0;
                int valor = 0;
                int cantidad = 0;
                int cantidadVieja = 0;
                int valorAntiguo = 0;
                int totalAntiguo = 0;
                int cantidadNueva = 0;
                int cantidadTotal = 0;
                if (int.Parse(txtCantidad.Text) > 0)
                {
                    foreach (var detalle_orden in listOrden)
                    {
                        if (detalle_orden.Codigo_barra == int.Parse(txtCodigoBarra.Text))
                        {
                            cantidad = int.Parse(txtCantidad.Text);
                            valor = int.Parse(txtValor.Text);
                            cantidadVieja = detalle_orden.Cantidad;
                            valorAntiguo = detalle_orden.Valor;
                            totalAntiguo = cantidadVieja * valorAntiguo;
                            montoTotal = montoTotal - totalAntiguo;
                            cantidadNueva = cantidad;
                            cantidadTotal = cantidadVieja + cantidadNueva;
                            detalle_orden.Cantidad = cantidadTotal;
                            total = total + (valor * cantidadTotal);
                            detalle_orden.Valor = valor;
                            montoTotal = montoTotal + total;
                            dgDetalleOrden.ItemsSource = null;
                            dgDetalleOrden.ItemsSource = listOrden;
                            txtMontoTotal.Text = montoTotal.ToString();
                            txtCodigoBarra.Text = 0.ToString();
                            txtNombreProducto.Text = string.Empty;
                            txtCantidad.Text = 0.ToString();
                            txtValor.Text = 0.ToString();
                            productoEnlista = 1;
                        }
                    }
                    if (productoEnlista == 0)
                    {
                        cantidad = int.Parse(txtCantidad.Text);
                        valor = int.Parse(txtValor.Text);
                        total = total + (valor * cantidad);
                        montoTotal = montoTotal + total;
                        Detalle_Orden detalle_orden = new Detalle_Orden(int.Parse(txtIdOrdenPedidos.Text), int.Parse(txtCodigoBarra.Text), txtNombreProducto.Text, int.Parse(txtCantidad.Text), int.Parse(txtValor.Text));
                        listOrden.Add(detalle_orden);
                        dgDetalleOrden.ItemsSource = null;
                        dgDetalleOrden.ItemsSource = listOrden;
                        txtMontoTotal.Text = montoTotal.ToString();
                        txtCodigoBarra.Text = 0.ToString();
                        txtNombreProducto.Text = string.Empty;
                        txtCantidad.Text = 0.ToString();
                        txtValor.Text = 0.ToString();
                    }
                }
                else
                {
                    await this.ShowMessageAsync("PRODUCTO", "La cantidad debe ser mayor a 0");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void btnModificarP_Click(object sender, RoutedEventArgs e)
        {
            int productoEnLista = 0;

            try
            {
                if (int.Parse(txtCantidad.Text) > 0)
                {
                    foreach (var detalle_orden in listOrden)
                    {
                        if (detalle_orden.Codigo_barra == int.Parse(txtCodigoBarra.Text))
                        {
                            productoEnLista = 1;
                            montoTotal = montoTotal - (detalle_orden.Valor * detalle_orden.Cantidad);
                            detalle_orden.Nombre_producto = txtNombreProducto.Text;
                            detalle_orden.Valor = int.Parse(txtValor.Text);
                            detalle_orden.Cantidad = int.Parse(txtCantidad.Text);
                            montoTotal = montoTotal + (int.Parse(txtValor.Text) * int.Parse(txtCantidad.Text));
                            txtMontoTotal.Text = montoTotal.ToString();
                            dgDetalleOrden.ItemsSource = null;
                            dgDetalleOrden.ItemsSource = listOrden;
                            txtCodigoBarra.Text = 0.ToString();
                            txtNombreProducto.Text = string.Empty;
                            txtCantidad.Text = 0.ToString();
                            txtValor.Text = 0.ToString();
                        }
                    }
                    if (productoEnLista == 0)
                    {
                        await this.ShowMessageAsync("PRODUCTO", "Producto no esta agregado");
                    }
                }
                else
                {
                    await this.ShowMessageAsync("PRODUCTO", "La cantidad debe ser mayor a 0");
                }

            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VisualizarOrdenDePedido vp = new VisualizarOrdenDePedido(nomUsuario);
            vp.Show();
            this.Close();
        }

        private async void GuardarDetalle(int cantidad, int orden, string nombre, int cod_barra, int valor)
        {
            try
            {
                OracleCommand cmd2 = new OracleCommand("sp_insertar_detalle_orden", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("cantidad", OracleDbType.Int32, 20).Value = cantidad;
                cmd2.Parameters.Add("orden", OracleDbType.Int32, 20).Value = orden;
                cmd2.Parameters.Add("nombre", OracleDbType.Varchar2, 100).Value = nombre;
                cmd2.Parameters.Add("cod_barra", OracleDbType.Int32, 20).Value = cod_barra;
                cmd2.Parameters.Add("valor", OracleDbType.Int32, 20).Value = valor;
                cmd2.ExecuteNonQuery();
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }

        private async void btnEliminarP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int listaP = 0;
                foreach (var detalleO in listOrden)
                {
                    if (detalleO.Codigo_barra == int.Parse(txtCodigoBarra.Text))
                    {
                        listaP = 1;
                        montoTotal = montoTotal - (detalleO.Valor * detalleO.Cantidad);
                        listOrden.Remove(detalleO);
                        dgDetalleOrden.ItemsSource = null;
                        dgDetalleOrden.ItemsSource = listOrden;
                        txtCodigoBarra.Text = 0.ToString();
                        txtNombreProducto.Text = string.Empty;
                        txtCantidad.Text = 0.ToString();
                        txtValor.Text = 0.ToString();
                        txtMontoTotal.Text = montoTotal.ToString();
                        await this.ShowMessageAsync("PRODUCTO", "Producto eliminado de la lista");
                        break;
                    }
                }
                if (listaP == 0)
                {
                    await this.ShowMessageAsync("PRODUCTO", "Producto no esta agregado");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }    
    }
}
