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
using BibliotecaLosYuyitos;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para IngresarRecepcion.xaml
    /// </summary>
    public partial class IngresarRecepcion 
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        DateTime fecha = DateTime.Today;
        List<Detalle_Recepcion> listRecepcion = new List<Detalle_Recepcion>();
        int montoTotal = 0;
        int verificarOrden = 0;
        int idOrden = 0;
        public IngresarRecepcion(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            nomUsuario = usuario;
            DatosUsuarios();
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
            ControlarRecepciones cr = new ControlarRecepciones(nomUsuario);
            cr.Show();
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

        private async void GenerarIdRecepcion()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(MAX(ID_RECEPCION), 7000) AS RECEPCION FROM CONTROL_RECEP";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int nro_recepcion = int.Parse(reader["RECEPCION"].ToString());
                    nro_recepcion = nro_recepcion + 1;
                    txtIdRecep.Text = nro_recepcion.ToString();
                }
                else
                {
                    await this.ShowMessageAsync("RECEPCION", "No se ha podido obtener el número de la recepción");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void btnVerificarOrden_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                idOrden = int.Parse(txtIdOrdenPedidoR.Text);
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT FECH_ORDEN, MONTO_ORDEN, PROVEEDOR_RUT_PROVEE AS PROVEEDOR, DESCRIP_ORDEN FROM ORDEN_PED WHERE ID_ORDEN = :id";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("id", OracleDbType.Int32, 20).Value = idOrden;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    verificarOrden = 1;
                    dpFechaOrden.Text = reader["FECH_ORDEN"].ToString();
                    txtMontoO.Text = reader["MONTO_ORDEN"].ToString();
                    txtProveedorO.Text = reader["PROVEEDOR"].ToString();
                    txtProveedor.Text = reader["PROVEEDOR"].ToString();
                    txtDescripcion.Text = reader["DESCRIP_ORDEN"].ToString();
                    OracleCommand cmd2 = con.CreateCommand();
                    cmd2.CommandText = "SELECT PRODUCTO_COD_BARRA_PRODUCT AS CODIGO_BARRA, NOM_PRODUCT AS NOMBRE, CANTI_PRODUCT AS CANTIDAD, VALOR FROM DETALLE_ORDEN WHERE ORDEN_PED_ID_ORDEN = :id";
                    cmd2.CommandType = CommandType.Text;
                    cmd2.Parameters.Add("id", OracleDbType.Int32, 20).Value = idOrden;
                    OracleDataReader dr = cmd2.ExecuteReader();
                    if (dr.Read())
                    {
                        OracleDataAdapter adapter = new OracleDataAdapter(cmd2);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgOrden.ItemsSource = dt.DefaultView;
                        dr.Close();
                        await this.ShowMessageAsync("ORDEN DE PEDIDO", "Orden de pedido obtenida correctamente");
                    }
                    else
                    {
                        await this.ShowMessageAsync("ORDEN DE PEDIDO", "Detalle de orden no obtenido");
                    }
                }
                else
                {
                    await this.ShowMessageAsync("ORDEN DE PEDIDO", "Orden no encontrada");
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
                    foreach (var detalle_recep in listRecepcion)
                    {
                        if (detalle_recep.Codigo_barra == int.Parse(txtCodigoBarra.Text))
                        {
                            cantidad = int.Parse(txtCantidad.Text);
                            valor = int.Parse(txtValor.Text);
                            cantidadVieja = detalle_recep.Cantidad;
                            valorAntiguo = detalle_recep.Valor;
                            totalAntiguo = cantidadVieja * valorAntiguo;
                            montoTotal = montoTotal - totalAntiguo;
                            cantidadNueva = cantidad;
                            cantidadTotal = cantidadVieja + cantidadNueva;
                            detalle_recep.Cantidad = cantidadTotal;
                            total = total + (valor * cantidadTotal);
                            detalle_recep.Valor = valor;
                            montoTotal = montoTotal + total;
                            dgDetalleR.ItemsSource = null;
                            dgDetalleR.ItemsSource = listRecepcion;
                            txtValorAPagar.Text = montoTotal.ToString();
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
                        Detalle_Recepcion detalle_Recepcion = new Detalle_Recepcion(int.Parse(txtIdRecep.Text), int.Parse(txtCodigoBarra.Text), txtNombreProducto.Text, int.Parse(txtCantidad.Text), int.Parse(txtValor.Text));
                        listRecepcion.Add(detalle_Recepcion);
                        dgDetalleR.ItemsSource = null;
                        dgDetalleR.ItemsSource = listRecepcion;
                        txtValorAPagar.Text = montoTotal.ToString();
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
                if(int.Parse(txtCantidad.Text) > 0)
                {
                    foreach (var detalle_recep in listRecepcion)
                    {
                        if (detalle_recep.Codigo_barra == int.Parse(txtCodigoBarra.Text))
                        {
                            productoEnLista = 1;
                            montoTotal = montoTotal - (detalle_recep.Valor * detalle_recep.Cantidad);
                            detalle_recep.Nombre_producto = txtNombreProducto.Text;
                            detalle_recep.Valor = int.Parse(txtValor.Text);
                            detalle_recep.Cantidad = int.Parse(txtCantidad.Text);
                            montoTotal = montoTotal + (int.Parse(txtValor.Text) * int.Parse(txtCantidad.Text));
                            txtValorAPagar.Text = montoTotal.ToString();
                            dgDetalleR.ItemsSource = null;
                            dgDetalleR.ItemsSource = listRecepcion;
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

        private async void btnGuardarRecepcion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (verificarOrden == 1)
                {
                    if(txtProveedor.Text == txtProveedorO.Text)
                    {
                        if (listRecepcion.Count() > 0)
                        {
                            OracleCommand cmd = new OracleCommand("sp_insertar_recepcion", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("id", OracleDbType.Int32, 20).Value = int.Parse(txtIdRecep.Text);
                            cmd.Parameters.Add("valor", OracleDbType.Int32, 20).Value = int.Parse(txtValorAPagar.Text);
                            cmd.Parameters.Add("fecha", OracleDbType.Date).Value = dpFechaRecepcion.SelectedDate;
                            cmd.Parameters.Add("orden", OracleDbType.Int32, 20).Value = idOrden;
                            int n = cmd.ExecuteNonQuery();
                            if (n < 0)
                            {
                                foreach (var detalle in listRecepcion)
                                {
                                    GuardarDetalle(detalle.Cantidad, detalle.Id_recepcion, detalle.Nombre_producto, detalle.Codigo_barra, detalle.Valor);
                                }
                                await this.ShowMessageAsync("Recepción", "Recepción realizada");
                                resetAll();
                            }
                            else
                            {
                                await this.ShowMessageAsync("Recepción", "Recepción no realizada");
                            }
                        }
                        else
                        {
                            await this.ShowMessageAsync("Recepción", "No hay productos agregado a la lista");
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("Recepción", "El rut del proveedor ingresado no es el mismo de la orden de pedido");
                    }
                    
                }
                else
                {
                    await this.ShowMessageAsync("ORDEN DE PEDIDO", "Orden de pedido no esta verificada");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void GuardarDetalle(int cantidad, int recepcion, string nombre, int cod_barra, int valor)
        {
            try
            {
                OracleCommand cmd2 = new OracleCommand("sp_insertar_detalle_recep", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("cantidad", OracleDbType.Int32, 20).Value = cantidad;
                cmd2.Parameters.Add("recepcion", OracleDbType.Int32, 20).Value = recepcion;
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
                foreach (var detalleR in listRecepcion)
                {
                    if (detalleR.Codigo_barra == int.Parse(txtCodigoBarra.Text))
                    {
                        MessageDialogResult respuesta = await this.ShowMessageAsync("ELIMINAR", "¿Desea Eliminar el producto de la lista?", MessageDialogStyle.AffirmativeAndNegative);

                        if (respuesta == MessageDialogResult.Affirmative)
                        {
                            listaP = 1;
                            montoTotal = montoTotal - (detalleR.Valor * detalleR.Cantidad);
                            listRecepcion.Remove(detalleR);
                            dgDetalleR.ItemsSource = null;
                            dgDetalleR.ItemsSource = listRecepcion;
                            txtCodigoBarra.Text = 0.ToString();
                            txtNombreProducto.Text = string.Empty;
                            txtCantidad.Text = 0.ToString();
                            txtValor.Text = 0.ToString();
                            txtValorAPagar.Text = montoTotal.ToString();
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

        private void resetAll()
        {
            GenerarIdRecepcion();
            txtValorAPagar.Text = 0.ToString();
            txtCantidad.Text = 0.ToString();
            txtCodigoBarra.Text = 0.ToString();
            txtNombreProducto.Text = string.Empty;
            dpFechaRecepcion.SelectedDate = fecha;
            txtIdOrdenPedidoR.Text = 0.ToString();
            txtValor.Text = 0.ToString();
            txtProveedor.Text = string.Empty;
            for (int i = listRecepcion.Count - 1; i >= 0; i--)
            {
                listRecepcion.RemoveAt(i);
            }
            dgDetalleR.ItemsSource = null;
            dgDetalleR.ItemsSource = listRecepcion;
            txtMontoO.Text = 0.ToString();
            txtProveedorO.Text = string.Empty;
            dpFechaOrden.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
            dgOrden.ItemsSource = null;
            montoTotal = 0;

        }

        private void btnLimpiarCampos_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }
    }
}
