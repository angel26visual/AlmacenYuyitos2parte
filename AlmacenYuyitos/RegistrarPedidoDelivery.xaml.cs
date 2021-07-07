using BibliotecaLosYuyitos;
using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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
using static BibliotecaLosYuyitos.Delivery;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para RegistrarPedidoDelivery.xaml
    /// </summary>
    public partial class RegistrarPedidoDelivery
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        List<Detalle_boleta> listDboleta = new List<Detalle_boleta>();
        DateTime fecha = DateTime.Today;
        int monto_total = 0;
        int monto_descuento = 0;
        string rutTrab = "";
        bool validaciones = true;
        public RegistrarPedidoDelivery(string usuario)
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
            cmd.CommandText = "SELECT NOMBRE_TRAB, APELLIDO_TRAB, CARGO_TRABAJADOR_ID_CARGO, RUT_TRAB FROM TRABAJADOR WHERE NOM_USUARIO = :USUARIO";
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
                    rutTrab = reader["RUT_TRAB"].ToString();
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

        private async void setConnection()
        {
            String connectionString = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            con = new OracleConnection(connectionString);
            try
            {
                con.Open();
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "No hay conexión con la base de datos");
            }
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarDelivery gd = new GestionarDelivery(nomUsuario);
            gd.Show();
            this.Close();
        }

        private async void btnGuardarPedidoDelivery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Validar();
                if (validaciones)
                {
                    int boleta = int.Parse(txtNumeroBoleta.Text);
                    OracleCommand cmd = new OracleCommand("sp_insertar_delivery", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("boleta", OracleDbType.Int32, 20).Value = int.Parse(txtNumeroBoleta.Text);
                    cmd.Parameters.Add("fecha", OracleDbType.Date).Value = txtFechaVenta.SelectedDate;
                    cmd.Parameters.Add("venta", OracleDbType.Int32, 30).Value = monto_total + monto_descuento + int.Parse(txtValorDespacho.Text);
                    cmd.Parameters.Add("descuento", OracleDbType.Int32, 30).Value = monto_descuento;
                    cmd.Parameters.Add("monto", OracleDbType.Int32, 30).Value = monto_total;
                    cmd.Parameters.Add("pago", OracleDbType.Int32, 20).Value = int.Parse(txtPago.Text);


                    cmd.Parameters.Add("rut", OracleDbType.Varchar2, 20).Value = rutTrab;


                    cmd.Parameters.Add("medio_pago", OracleDbType.Int32, 20).Value = cboMedioPago.SelectedValue;
                    cmd.Parameters.Add("entrega", OracleDbType.Date).Value = dpFechaeEntrega.SelectedDate;

                    if (txtNombreCliente.Text.Replace(" ", string.Empty).Length >= 3)
                    {
                        cmd.Parameters.Add("cliente", OracleDbType.Varchar2, 100).Value = txtNombreCliente.Text;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "El nombre del Cliente debe tener mínimo 3 caracteres!");
                        return;

                    }
                    if (txtDireccionDelivery.Text.Replace(" ", string.Empty).Length >= 3)
                    {
                        cmd.Parameters.Add("direccion", OracleDbType.Varchar2, 100).Value = txtDireccionDelivery.Text;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "La Dirección del Cliente debe tener mínimo 3 caracteres!");
                        return;

                    }

                    if (txtTelefonoContacto.Text.Replace(" ", string.Empty).Length == 9)
                    {
                        cmd.Parameters.Add("fono", OracleDbType.Int32, 20).Value = txtTelefonoContacto.Text;
                    }
                    else
                    {
                        await this.ShowMessageAsync("Error", "el Teléfono debe tener 9 dígitos!");
                        return;
                    }
                    int n = cmd.ExecuteNonQuery();
                    if (n < 0)
                    {
                        foreach (var detalle_Boleta in listDboleta)
                        {
                            GuardarDetalle(detalle_Boleta.Nro_boleta, detalle_Boleta.Codigo_producto, detalle_Boleta.Cantidad);
                        }
                        int vuelto = int.Parse(txtPago.Text) - monto_total;
                        if (vuelto > 0)
                        {
                            await this.ShowMessageAsync("PEDIDO REALIZADO", "El vuelto del cliente es:" + " $ " + vuelto + " " + "pesos");
                            resetAll();
                        }
                        else
                        {
                            await this.ShowMessageAsync("PEDIDO REALIZADO", "Pedido sin vuelto al cliente");
                            resetAll();
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("PEDIDO", "Pedido no realizado");
                    }
                }
                
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void GuardarDetalle(int boleta, int producto, int cantidad)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_insertar_detalle_boleta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("boleta", OracleDbType.Int32, 20).Value = boleta;
                cmd.Parameters.Add("producto", OracleDbType.Int32, 20).Value = producto;
                cmd.Parameters.Add("cantidad", OracleDbType.Int32, 20).Value = cantidad;
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }
        private async void btnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int precio = 0;
                int tipo = 0;
                int total = 0;
                int cantidad = 0;
                string nombre = "";
                int cantidadVieja = 0;
                int cantidadNueva = 0;
                int productoEnlista = 0;
                if (int.Parse(txtCantidad.Text) > 0)
                {
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = "SELECT STOCK, NOMBRE_PRODUCT, PRECIO_VENTA, TIPO_PRODUCTO_ID_TIPPRODUC AS TIPO FROM PRODUCTO WHERE CODIGO_PRODUCTO = :CODIGO";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("CODIGO", OracleDbType.Int32, 40).Value = int.Parse(txtCodigoProducto.Text);
                    OracleDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        cantidad = int.Parse(txtCantidad.Text);
                        nombre = reader["NOMBRE_PRODUCT"].ToString();
                        precio = int.Parse(reader["PRECIO_VENTA"].ToString());
                        tipo = int.Parse(reader["TIPO"].ToString());
                        foreach (var detalle_boleta in listDboleta)
                        {
                            if (detalle_boleta.Codigo_producto == int.Parse(txtCodigoProducto.Text))
                            {
                                productoEnlista = 1;
                                cantidad = detalle_boleta.Cantidad + cantidad;
                                if (int.Parse(reader["STOCK"].ToString()) >= cantidad)
                                {
                                    cantidadVieja = detalle_boleta.Cantidad;
                                    cantidadNueva = cantidad;
                                    cantidad = cantidadNueva + cantidadVieja;
                                    detalle_boleta.Cantidad = cantidad;
                                    total = total + (precio * cantidad);
                                    monto_total = monto_total + total;
                                    txtTotalDelivery.Text = monto_total.ToString();
                                    dgDelivery.ItemsSource = null;
                                    dgDelivery.ItemsSource = listDboleta;
                                    txtCodigoProducto.Text = 0.ToString();
                                    txtCantidad.Text = 0.ToString();
                                    DesCalcularPromocionCantidad(tipo, precio, cantidadVieja);
                                    CalcularPromocionCantidad(tipo, precio, cantidadNueva);
                                }
                                else
                                {
                                    await this.ShowMessageAsync("PRODUCTO", "Producto con stock insuficiente");
                                }
                            }
                        }
                        if (productoEnlista == 0)
                        {
                            if (int.Parse(reader["STOCK"].ToString()) >= int.Parse(txtCantidad.Text))
                            {
                                nombre = reader["NOMBRE_PRODUCT"].ToString();
                                precio = int.Parse(reader["PRECIO_VENTA"].ToString());
                                total = total + (precio * int.Parse(txtCantidad.Text));
                                tipo = int.Parse(reader["TIPO"].ToString());
                                Detalle_boleta detalle_Boleta = new Detalle_boleta(int.Parse(txtNumeroBoleta.Text), int.Parse(txtCodigoProducto.Text), nombre, int.Parse(txtCantidad.Text), precio, tipo);
                                listDboleta.Add(detalle_Boleta);
                                dgDelivery.ItemsSource = null;
                                dgDelivery.ItemsSource = listDboleta;
                                cantidadNueva = int.Parse(txtCantidad.Text);
                                CalcularPromocionTipo(tipo, precio);
                                CalcularPromocionCantidad(tipo, precio, cantidadNueva);
                                monto_total = monto_total + total;
                                txtTotalDelivery.Text = monto_total.ToString();
                                txtCodigoProducto.Text = 0.ToString();
                                txtCantidad.Text = 0.ToString();
                            }
                            else
                            {
                                await this.ShowMessageAsync("PRODUCTO", "Producto con stock insuficiente");
                            }
                        }

                    }
                    else
                    {
                        await this.ShowMessageAsync("PRODUCTO", "El producto no esta registrado");
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

        private void btnModificarCantidad_Click(object sender, RoutedEventArgs e)
        {
            Modificar(int.Parse(txtCodigoProducto.Text));
        }

        

        public async void Modificar(int codigo)
        {
            int precio = 0;
            int tipo = 0;
            int total = 0;
            int cantidad = 0;
            int cantidadVieja = 0;
            int cantidadNueva = 0;
            int productoEnLista = 0;
            try
            {
                foreach (var detalle_boleta in listDboleta)
                {
                    if (detalle_boleta.Codigo_producto == codigo)
                    {
                        productoEnLista = 1;
                        if (int.Parse(txtCantidad.Text) > 0)
                        {
                            OracleCommand cmd = con.CreateCommand();
                            cmd.CommandText = "SELECT STOCK, NOMBRE_PRODUCT, PRECIO_VENTA, TIPO_PRODUCTO_ID_TIPPRODUC AS TIPO FROM PRODUCTO WHERE CODIGO_PRODUCTO = :CODIGO";
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.Add("CODIGO", OracleDbType.Int32, 40).Value = int.Parse(txtCodigoProducto.Text);
                            OracleDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                cantidad = int.Parse(txtCantidad.Text);

                                precio = int.Parse(reader["PRECIO_VENTA"].ToString());
                                tipo = int.Parse(reader["TIPO"].ToString());
                                if (int.Parse(reader["STOCK"].ToString()) >= cantidad)
                                {
                                    detalle_boleta.Cantidad = cantidad;
                                    cantidadVieja = detalle_boleta.Cantidad;
                                    cantidadNueva = int.Parse(txtCantidad.Text);
                                    cantidad = cantidadNueva - cantidadVieja;
                                    total = total + (precio * cantidad);
                                    monto_total = monto_total + total;
                                    txtTotalDelivery.Text = monto_total.ToString();
                                    dgDelivery.ItemsSource = null;
                                    dgDelivery.ItemsSource = listDboleta;
                                    DesCalcularPromocionCantidad(tipo, precio, cantidadVieja);
                                    CalcularPromocionCantidad(tipo, precio, cantidadNueva);
                                    txtCantidad.Text = 0.ToString();
                                    txtCodigoProducto.Text = 0.ToString();
                                }
                                else
                                {
                                    await this.ShowMessageAsync("PRODUCTO", "Producto con stock insuficiente");
                                }
                            }
                        }
                    }
                }
                if (productoEnLista == 0)
                {
                    await this.ShowMessageAsync("PRODUCTO", "Producto no esta agregado");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void CalcularPromocionTipo(int tipo, int precio)
        {
            int porcentaje = 0;
            int efectivo = 0;
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(SUM(DESCUENTO_PORCENTAJE)/100, 0) AS PORCENTAJE, NVL(SUM(DESCUENTO_EFECTIVO), 0) AS EFECTIVO FROM PROMOCION WHERE FECHA_INICIO_PROMO <= :FECHA_INICIO AND FECHA_FIN_PROMO >= :FECHA_FIN AND TIPO_PRODUCTO_ID_TIPPRODUC = :TIPO_PRODUCTO AND TIPO_PROMOCION_ID_TIPOPROMO = 1";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                cmd.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                cmd.Parameters.Add("TIPO_PRODUCTO", OracleDbType.Int32, 40).Value = tipo;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    porcentaje = porcentaje + (precio * int.Parse(reader["PORCENTAJE"].ToString()));
                    efectivo = efectivo + int.Parse(reader["EFECTIVO"].ToString());
                }
                monto_descuento = monto_descuento + (porcentaje + efectivo);
                txtTotalDescuentos.Text = monto_descuento.ToString();
                monto_total = monto_total - (porcentaje + efectivo);
                txtTotalDelivery.Text = monto_total.ToString();

            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }
        private async void CalcularPromocionCantidad(int tipo, int precio, int cantidadNueva)
        {
            int porcentaje = 0;
            int efectivo = 0;
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(SUM(DESCUENTO_PORCENTAJE)/100, 0) AS PORCENTAJE, NVL(SUM(DESCUENTO_EFECTIVO), 0) AS EFECTIVO FROM PROMOCION WHERE FECHA_INICIO_PROMO <= :FECHA_INICIO AND FECHA_FIN_PROMO >= :FECHA_FIN AND TIPO_PRODUCTO_ID_TIPPRODUC = :TIPO_PRODUCTO AND CANT_PRODUCTO >= :CANTIDAD AND TIPO_PROMOCION_ID_TIPOPROMO = 2";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                cmd.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                cmd.Parameters.Add("TIPO_PRODUCTO", OracleDbType.Int32, 40).Value = tipo;
                cmd.Parameters.Add("CANTIDAD", OracleDbType.Int32, 40).Value = cantidadNueva;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    porcentaje = porcentaje + (precio * int.Parse(reader["PORCENTAJE"].ToString()));
                    efectivo = efectivo + int.Parse(reader["EFECTIVO"].ToString());
                }
                OracleCommand cmd3 = con.CreateCommand();
                cmd3.CommandText = "SELECT NVL(SUM(DESCUENTO_PORCENTAJE)/100, 0) AS PORCENTAJE, NVL(SUM(DESCUENTO_EFECTIVO), 0) AS EFECTIVO FROM PROMOCION WHERE FECHA_INICIO_PROMO <= :FECHA_INICIO AND FECHA_FIN_PROMO >= :FECHA_FIN AND CANT_PRODUCTO >= :CANTIDAD AND TIPO_PROMOCION_ID_TIPOPROMO = 2 AND TIPO_PRODUCTO_ID_TIPPRODUC = NULL";
                cmd3.CommandType = CommandType.Text;
                cmd3.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                cmd3.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                cmd3.Parameters.Add("CANTIDAD", OracleDbType.Int32, 40).Value = cantidadNueva;
                OracleDataReader reader3 = cmd3.ExecuteReader();
                if (reader3.Read())
                {
                    porcentaje = porcentaje + (precio * int.Parse(reader3["PORCENTAJE"].ToString()));
                    efectivo = efectivo + int.Parse(reader3["EFECTIVO"].ToString());
                }
                monto_descuento = monto_descuento + (porcentaje + efectivo);
                txtTotalDescuentos.Text = monto_descuento.ToString();
                monto_total = monto_total - (porcentaje + efectivo);
                txtTotalDelivery.Text = monto_total.ToString();

            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void DesCalcularPromocionCantidad(int tipo, int precio, int cantidadVieja)
        {
            int porcentaje = 0;
            int efectivo = 0;
            int descuento = 0;
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(SUM(DESCUENTO_PORCENTAJE)/100, 0) AS PORCENTAJE, NVL(SUM(DESCUENTO_EFECTIVO), 0) AS EFECTIVO FROM PROMOCION WHERE FECHA_INICIO_PROMO <= :FECHA_INICIO AND FECHA_FIN_PROMO >= :FECHA_FIN AND TIPO_PRODUCTO_ID_TIPPRODUC = :TIPO_PRODUCTO AND CANT_PRODUCTO >= :CANTIDAD AND TIPO_PROMOCION_ID_TIPOPROMO = 2";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                cmd.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                cmd.Parameters.Add("TIPO_PRODUCTO", OracleDbType.Int32, 40).Value = tipo;
                cmd.Parameters.Add("CANTIDAD", OracleDbType.Int32, 40).Value = cantidadVieja;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    porcentaje = porcentaje + (precio * int.Parse(reader["PORCENTAJE"].ToString()));
                    efectivo = efectivo + int.Parse(reader["EFECTIVO"].ToString());
                }
                OracleCommand cmd3 = con.CreateCommand();
                cmd3.CommandText = "SELECT NVL(SUM(DESCUENTO_PORCENTAJE)/100, 0) AS PORCENTAJE, NVL(SUM(DESCUENTO_EFECTIVO), 0) AS EFECTIVO FROM PROMOCION WHERE FECHA_INICIO_PROMO <= :FECHA_INICIO AND FECHA_FIN_PROMO >= :FECHA_FIN AND CANT_PRODUCTO >= :CANTIDAD AND TIPO_PROMOCION_ID_TIPOPROMO = 2 AND TIPO_PRODUCTO_ID_TIPPRODUC = NULL";
                cmd3.CommandType = CommandType.Text;
                cmd3.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                cmd3.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                cmd3.Parameters.Add("CANTIDAD", OracleDbType.Int32, 40).Value = cantidadVieja;
                OracleDataReader reader3 = cmd3.ExecuteReader();
                if (reader3.Read())
                {
                    porcentaje = porcentaje + (precio * int.Parse(reader3["PORCENTAJE"].ToString()));
                    efectivo = efectivo + int.Parse(reader3["EFECTIVO"].ToString());
                }
                descuento = porcentaje + efectivo;
                monto_descuento = monto_descuento - descuento;
                txtTotalDescuentos.Text = monto_descuento.ToString();
                monto_total = monto_total + descuento;
                txtTotalDelivery.Text = monto_total.ToString();
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void GenerarNroBoleta()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT MAX(NRO_BOLETA) FROM BOLETA";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    int nro_boleta = int.Parse(reader["MAX(NRO_BOLETA)"].ToString());
                    nro_boleta = nro_boleta + 1;
                    txtNumeroBoleta.Text = nro_boleta.ToString();
                }
                else
                {
                    await this.ShowMessageAsync("BOLETA", "No se ha podido obtener el número de la boleta");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }

        private async void cboMedioPago_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_MEDIOPAGO , DESCRIP_MEDIOPAGO FROM MEDIO_PAGO";
                cmd.CommandType = CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboMedioPago.ItemsSource = dt.AsDataView();
                cboMedioPago.DisplayMemberPath = "DESCRIP_MEDIOPAGO";
                cboMedioPago.SelectedValuePath = "ID_MEDIOPAGO";
                cboMedioPago.SelectedValue = 1;
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
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

        private async void txtNombreCliente_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Text, "^[a-zA-Z]"))
            {
                e.Handled = true;
                await this.ShowMessageAsync("Error", "El Nombre del Cliente debe contener sólo letras");
            }
        }

        private async void txtCodigoProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Código del Producto debe contener sólo números");
            }
        }

        private async void txtCantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "La cantidad de producto debe contener sólo números");
            }
        }

        private async void txtTelefonoContacto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Teléfono de Contacto del Cliente debe contener sólo números");
            }
        }

        private async void txtValorDespacho_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Valor de Despacho debe contener sólo números");
            }
        }

        private async void txtPago_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Monto de Pago debe contener sólo números");
            }
        }

        private void btnLimpiarCampos_Click(object sender, RoutedEventArgs e)
        {
            resetAll();
        }

        private void resetAll()
        {
            txtCodigoProducto.Text = 0.ToString();
            txtCantidad.Text = 0.ToString();
            txtFechaVenta.SelectedDate = fecha;
            dpFechaeEntrega.SelectedDate = fecha;
            txtNombreCliente.Text = string.Empty;
            txtDireccionDelivery.Text = string.Empty;
            txtTelefonoContacto.Text = 0.ToString();
            txtPago.Text = 0.ToString();
            GenerarNroBoleta();
            txtTotalDescuentos.Text = 0.ToString();
            txtTotalDelivery.Text = 0.ToString();
            txtValorDespacho.Text = 5000.ToString();
            for (int i = listDboleta.Count() - 1; i >= 0; i--)
            {
                listDboleta.RemoveAt(i);
            }
            dgDelivery.ItemsSource = null;
            dgDelivery.ItemsSource = listDboleta;
            monto_total = 0;
            monto_descuento = 0;
            cboMedioPago.SelectedValue = 1;
        }

        private async void Validar()
        {
            try
            {
                int validar = 0;
                if (listDboleta.Count() <= 0)
                {
                    validar = 1;
                    await this.ShowMessageAsync("VENTA", "No hay productos agregados a la lista");
                }
                if (Convert.ToInt32(txtPago.Text) < 0 && validar == 0)
                {
                    validar = 1;
                    await this.ShowMessageAsync("VENTA", "El monto de pago no puede ser menor a 0");
                }

                if (validar == 0)
                {
                    validaciones = true;
                }
                else if (validar == 1)
                {
                    validaciones = false;
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
                int Enlista = 0;
                foreach (var boleta in listDboleta)
                {
                    if (boleta.Codigo_producto == int.Parse(txtCodigoProducto.Text))
                    {
                        Enlista = 1;
                        monto_total = monto_total - (boleta.Precio * boleta.Cantidad);
                        DescalcularPromocionTipoE(boleta.Tipo, boleta.Precio);
                        DesCalcularPromocionCantidad(boleta.Tipo, boleta.Precio, boleta.Cantidad);
                        listDboleta.Remove(boleta);
                        dgDelivery.ItemsSource = null;
                        dgDelivery.ItemsSource = listDboleta;
                        txtCodigoProducto.Text = 0.ToString();
                        txtCantidad.Text = 0.ToString();
                        await this.ShowMessageAsync("PRODUCTO", "Producto eliminado de la lista");
                        break;
                    }
                }
                if (Enlista == 0)
                {
                    await this.ShowMessageAsync("PRODUCTO", "Producto no esta agregado");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void DescalcularPromocionTipoE(int tipo, int precio)
        {
            int porcentaje = 0;
            int efectivo = 0;
            int descuento = 0;
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(SUM(DESCUENTO_PORCENTAJE)/100, 0) AS PORCENTAJE, NVL(SUM(DESCUENTO_EFECTIVO), 0) AS EFECTIVO FROM PROMOCION WHERE FECHA_INICIO_PROMO <= :FECHA_INICIO AND FECHA_FIN_PROMO >= :FECHA_FIN AND TIPO_PRODUCTO_ID_TIPPRODUC = :TIPO_PRODUCTO AND TIPO_PROMOCION_ID_TIPOPROMO = 1";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                cmd.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                cmd.Parameters.Add("TIPO_PRODUCTO", OracleDbType.Int32, 40).Value = tipo;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    porcentaje = porcentaje + (precio * int.Parse(reader["PORCENTAJE"].ToString()));
                    efectivo = efectivo + int.Parse(reader["EFECTIVO"].ToString());
                }
                descuento = porcentaje + efectivo;
                monto_descuento = monto_descuento - descuento;
                txtTotalDescuentos.Text = monto_descuento.ToString();
                monto_total = monto_total + descuento;
                txtTotalDelivery.Text = monto_total.ToString();

            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }
    }
}

