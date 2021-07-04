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
    /// Lógica de interacción para VerOrden.xaml
    /// </summary>
    public partial class VerOrden
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        List<Detalle_Orden> listOrden = new List<Detalle_Orden>();
        int montoTotal = 0;
        public VerOrden(string usuario, int orden, int total)
        {
            this.setConnection();
            InitializeComponent();
            nomUsuario = usuario;
            DatosUsuarios();
            CargaCboProveedor();
            txtDescripcion.Text = "";
            cboProveedor.SelectedValue = 0;
            txtCantidad.Text = 0.ToString();
            txtValor.Text = 0.ToString();
            txtCodigoBarra.Text = 0.ToString();
            txtNombreProducto.Text = "";
            montoTotal = total;
            CargarLista(orden);
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
            VisualizarOrdenDePedido ordenDePedido = new VisualizarOrdenDePedido(nomUsuario);
            ordenDePedido.Show();
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

        private async void btnEliminarP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int listaP = 0;
                foreach (var detalleO in listOrden)
                {
                    if (detalleO.Codigo_barra == int.Parse(txtCodigoBarra.Text))
                    {
                        MessageDialogResult respuesta = await this.ShowMessageAsync("ELIMINAR", "¿Desea Eliminar el producto de la lista?", MessageDialogStyle.AffirmativeAndNegative);

                        if (respuesta == MessageDialogResult.Affirmative)
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
                        else
                        {
                            break;
                        }
                            
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

        private async void btnModificarOrdenPedido_Click(object sender, RoutedEventArgs e)
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

                if (txtDescripcion.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("descripcion", OracleDbType.Varchar2, 100).Value = txtDescripcion.Text;

                }
                else
                {
                    await this.ShowMessageAsync("Error", "La descripción de la Orden debe tener mas de 3 caracteres!");
                    return;

                }
                int n = cmd.ExecuteNonQuery();
                if (n < 0)
                {
                    EliminarDetalle();
                    foreach (var detalleO in listOrden)
                    {
                        GuardarDetalle(detalleO.Cantidad, detalleO.Id_orden, detalleO.Nombre_producto, detalleO.Codigo_barra, detalleO.Valor);
                    }
                    await this.ShowMessageAsync("actualizada", "Orden Actualizada Exitosamente");

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

        private async void btnEliminarOrdenPedido_Click(object sender, RoutedEventArgs e)
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
                        await this.ShowMessageAsync("ORDEN DE PEDIDO", "Orden de Pedido Eliminada correctamente");
                        VisualizarOrdenDePedido ordenDePedido = new VisualizarOrdenDePedido(nomUsuario);
                        ordenDePedido.Show();
                        this.Close();

                    }
                    else
                    {
                        await this.ShowMessageAsync("ORDEN DE PEDIDO", "Orden de Pedido NO Eliminada");
                    }

                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void CargarLista(int orden)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ORDEN_PED_ID_ORDEN, PRODUCTO_COD_BARRA_PRODUCT, NOM_PRODUCT, CANTI_PRODUCT, VALOR FROM DETALLE_ORDEN WHERE ORDEN_PED_ID_ORDEN = :orden";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("orden", OracleDbType.Int32, 20).Value = orden;
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Detalle_Orden detalle_orden = new Detalle_Orden(int.Parse(reader["ORDEN_PED_ID_ORDEN"].ToString()), int.Parse(reader["PRODUCTO_COD_BARRA_PRODUCT"].ToString()), reader["NOM_PRODUCT"].ToString(), int.Parse(reader["CANTI_PRODUCT"].ToString()), int.Parse(reader["VALOR"].ToString()));
                    listOrden.Add(detalle_orden);
                    dgDetalleOrden.ItemsSource = null;
                    dgDetalleOrden.ItemsSource = listOrden;

                }
                
            }
            catch (Exception)
            { 
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
            
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

        private async void EliminarDetalle()
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_eliminar_detalleO", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("id_orden", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdOrdenPedidos.Text);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }
    }
}
