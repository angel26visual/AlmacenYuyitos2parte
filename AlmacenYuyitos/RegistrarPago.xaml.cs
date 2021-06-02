using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Data;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para RegistrarPago.xaml
    /// </summary>
    public partial class RegistrarPago
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        List<Detalle_boleta> listDboleta = new List<Detalle_boleta>();
        DateTime fecha = DateTime.Today;
        int monto_total = 0;
        string rutTrab = string.Empty;

        public RegistrarPago(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            txtFechaVenta.SelectedDate = fecha;
            GenerarNroBoleta();
            txtTotalDescuento.Text = 0.ToString();
            txtTotalVenta.Text = 0.ToString();
            nomUsuario = usuario;
            DatosUsuarios();
        }

        private async void DatosUsuarios()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT RUT_TRAB, NOMBRE_TRAB, APELLIDO_TRAB, CARGO_TRABAJADOR_ID_CARGO FROM TRABAJADOR WHERE NOM_USUARIO = :USUARIO";
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
            GestionarVentas gv = new GestionarVentas(nomUsuario);
            gv.Show();
            this.Close();
        }

        private async void btnRealizarPago_Click(object sender, RoutedEventArgs e)
        {
            if (checkFiado.IsChecked == true)
            {
                if (VerificarClienteflyouts.IsOpen == true)
                {
                    VerificarClienteflyouts.IsOpen = false;
                }
                else
                {
                    VerificarClienteflyouts.IsOpen = true;
                }
            }
            else
            {
                try
                {
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = "INSERT INTO BOLETA(NRO_BOLETA, FECHA_VENTA, TOTAL_VENTA, TOTAL_DESCUENTOS, MONTO_TOTAL, MONTO_PAGO, TRABAJADOR_RUT_TRAB," +
                                       "TIPO_VENTA_ID_TIPVENTA, MEDIO_PAGO_ID_MEDIOPAGO) VALUES(:BOLETA, :FECHA, :TOTALVENTA, :TOTALDESCUENTO," +
                                       ":MONTO, :MONTOPAGO, :RUTTRAB, 1, :MEDIOPAGO)";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("BOLETA", OracleDbType.Int32, 20).Value = int.Parse(txtNroBoleta.Text);
                    cmd.Parameters.Add("FECHA", OracleDbType.Date).Value = txtFechaVenta.SelectedDate;
                    cmd.Parameters.Add("TOTALVENTA", OracleDbType.Int32, 30).Value = int.Parse(txtTotalVenta.Text);
                    cmd.Parameters.Add("TOTALDESCUENTO", OracleDbType.Int32, 30).Value = int.Parse(txtTotalDescuento.Text);
                    cmd.Parameters.Add("MONTO", OracleDbType.Int32, 30).Value = monto_total;
                    cmd.Parameters.Add("MONTOPAGO", OracleDbType.Int32, 20).Value = int.Parse(txtPago.Text);
                    cmd.Parameters.Add("RUTTRAB", OracleDbType.Varchar2, 20).Value = rutTrab;
                    cmd.Parameters.Add("MEDIOPAGO", OracleDbType.Int32, 20).Value = cboMedioPago.SelectedValue;
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("VENTA", "Venta realizada");
                    }
                    else
                    {
                        await this.ShowMessageAsync("VENTA", "Venta no realizada");
                    }
                }
                catch (Exception)
                {

                    await this.ShowMessageAsync("Error", "Ha ocurrido un error");
                }
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
                string nombre = null;
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
                                    detalle_boleta.Cantidad = cantidad;
                                    cantidad = cantidadNueva - cantidadVieja;
                                    total = total + (precio * cantidad);
                                    txtTotalVenta.Text = (int.Parse(txtTotalVenta.Text) + total).ToString();
                                    monto_total = monto_total + total;
                                    dgVerProductos.ItemsSource = null;
                                    dgVerProductos.ItemsSource = listDboleta;
                                    DesCalcularPromocionCantidad(tipo, precio, cantidadVieja, cantidadNueva);
                                    CalcularPromocionCantidad(tipo, precio, cantidadVieja, cantidadNueva);
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
                                Detalle_boleta detalle_Boleta = new Detalle_boleta(int.Parse(txtNroBoleta.Text), int.Parse(txtCodigoProducto.Text), nombre, int.Parse(txtCantidad.Text), precio);
                                listDboleta.Add(detalle_Boleta);
                                dgVerProductos.ItemsSource = null;
                                dgVerProductos.ItemsSource = listDboleta;
                                cantidadNueva = int.Parse(txtCantidad.Text);
                                CalcularPromocionTipo(tipo, precio);
                                CalcularPromocionCantidad(tipo, precio, cantidadVieja, cantidadNueva);
                                txtTotalVenta.Text = (int.Parse(txtTotalVenta.Text) + total).ToString();
                                monto_total = monto_total + total;
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
                txtTotalDescuento.Text = (int.Parse(txtTotalDescuento.Text) + (porcentaje + efectivo)).ToString();
                txtTotalVenta.Text = (int.Parse(txtTotalVenta.Text) - (porcentaje + efectivo)).ToString();
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }
        private async void CalcularPromocionCantidad(int tipo, int precio, int cantidadVieja, int cantidadNueva)
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
                    if (cantidadVieja > 0)
                    {
                        porcentaje = porcentaje + (precio * int.Parse(reader["PORCENTAJE"].ToString()));
                        efectivo = efectivo + int.Parse(reader["EFECTIVO"].ToString());
                    }
                    else
                    {
                        porcentaje = porcentaje + (precio * int.Parse(reader["PORCENTAJE"].ToString()));
                        efectivo = efectivo + int.Parse(reader["EFECTIVO"].ToString());
                    }

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
                    if (cantidadVieja > 0)
                    {
                        porcentaje = porcentaje + (precio * int.Parse(reader3["PORCENTAJE"].ToString()));
                        efectivo = efectivo + int.Parse(reader3["EFECTIVO"].ToString());
                    }
                    else
                    {
                        porcentaje = porcentaje + (precio * int.Parse(reader3["PORCENTAJE"].ToString()));
                        efectivo = efectivo + int.Parse(reader3["EFECTIVO"].ToString());
                    }

                }

                txtTotalDescuento.Text = (int.Parse(txtTotalDescuento.Text) + (porcentaje + efectivo)).ToString();
                txtTotalVenta.Text = (int.Parse(txtTotalVenta.Text) - (porcentaje + efectivo)).ToString();

            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void DesCalcularPromocionCantidad(int tipo, int precio, int cantidadVieja, int cantidadNueva)
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
                cmd.Parameters.Add("CANTIDAD", OracleDbType.Int32, 40).Value = cantidadVieja;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (cantidadVieja > 0)
                    {
                        porcentaje = porcentaje + (precio * int.Parse(reader["PORCENTAJE"].ToString()));
                        efectivo = efectivo + int.Parse(reader["EFECTIVO"].ToString());
                    }
                    else
                    {
                        porcentaje = porcentaje + (precio * int.Parse(reader["PORCENTAJE"].ToString()));
                        efectivo = efectivo + int.Parse(reader["EFECTIVO"].ToString());
                    }

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
                    if (cantidadVieja > 0)
                    {
                        porcentaje = porcentaje + (precio * int.Parse(reader3["PORCENTAJE"].ToString()));
                        efectivo = efectivo + int.Parse(reader3["EFECTIVO"].ToString());
                    }
                    else
                    {
                        porcentaje = porcentaje + (precio * int.Parse(reader3["PORCENTAJE"].ToString()));
                        efectivo = efectivo + int.Parse(reader3["EFECTIVO"].ToString());
                    }

                }

                txtTotalDescuento.Text = (int.Parse(txtTotalDescuento.Text) - (porcentaje + efectivo)).ToString();
                txtTotalVenta.Text = (int.Parse(txtTotalVenta.Text) + (porcentaje + efectivo)).ToString();

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
                    txtNroBoleta.Text = nro_boleta.ToString();
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
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private void btnEiminarProducto_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnModificarProducto_Click(object sender, RoutedEventArgs e)
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
            try
            {
                foreach (var detalle_boleta in listDboleta)
                {
                    if (detalle_boleta.Codigo_producto == codigo)
                    {
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
                                    cantidadVieja = detalle_boleta.Cantidad;
                                    cantidadNueva = int.Parse(txtCantidad.Text);
                                    detalle_boleta.Cantidad = cantidad;
                                    cantidad = cantidadNueva - cantidadVieja;
                                    total = total + (precio * cantidad);
                                    txtTotalVenta.Text = (int.Parse(txtTotalVenta.Text) + total).ToString();
                                    monto_total = monto_total + total;
                                    dgVerProductos.ItemsSource = null;
                                    dgVerProductos.ItemsSource = listDboleta;
                                    DesCalcularPromocionCantidad(tipo, precio, cantidadVieja, cantidadNueva);
                                    CalcularPromocionCantidad(tipo, precio, cantidadVieja, cantidadNueva);
                                }
                                else
                                {
                                    await this.ShowMessageAsync("PRODUCTO", "Producto con stock insuficiente");
                                }

                                detalle_boleta.Cantidad = int.Parse(txtCantidad.Text);
                            }
                        }
                    }
                    else
                    {
                        await this.ShowMessageAsync("PRODUCTO", "Producto que se intenta modificar no se ha agregado a la lista");
                    }
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }

        private async void btnVerificarCli_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NOMBRE_CLI FROM PRODUCTO WHERE CODIGO_PRODUCTO = :CODIGO";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("RUT", OracleDbType.Varchar2, 40).Value = txtRutCli.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtNombreCliente.Text = reader["NOMBRE_CLI"].ToString();
                }
                else
                {
                    await this.ShowMessageAsync("CLIENTE", "Cliente no encontrado");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }

        private async void btnVentaFiado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "INSERT INTO BOLETA(NRO_BOLETA, FECHA_VENTA, TOTAL_VENTA, TOTAL_DESCUENTOS, MONTO_TOTAL, MONTO_PAGO, TRABAJADOR_RUT_TRAB," +
                                   "TIPO_VENTA_ID_TIPVENTA, CLIENTE_RUT_CLI, ESTADO_DEUDA_ID_ESTADEUDA, NOM_CLIENTE, MEDIO_PAGO_ID_MEDIOPAGO) VALUES(:BOLETA, :FECHA, :TOTALVENTA, :TOTALDESCUENTO," +
                                   ":MONTO, :MONTOPAGO, :RUTTRAB, 2, :RUTCLI, 1, :NOMCLI, :MEDIOPAGO)";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("BOLETA", OracleDbType.Int32, 20).Value = int.Parse(txtNroBoleta.Text);
                cmd.Parameters.Add("FECHA", OracleDbType.Date).Value = txtFechaVenta.SelectedDate;
                cmd.Parameters.Add("TOTALVENTA", OracleDbType.Int32, 30).Value = int.Parse(txtTotalVenta.Text);
                cmd.Parameters.Add("TOTALDESCUENTO", OracleDbType.Int32, 30).Value = int.Parse(txtTotalDescuento.Text);
                cmd.Parameters.Add("MONTO", OracleDbType.Int32, 30).Value = monto_total;
                cmd.Parameters.Add("MONTOPAGO", OracleDbType.Int32, 20).Value = int.Parse(txtPago.Text);
                cmd.Parameters.Add("RUTTRAB", OracleDbType.Varchar2, 20).Value = rutTrab;
                cmd.Parameters.Add("RUTCLI", OracleDbType.Varchar2, 40).Value = txtRutCli.Text;
                cmd.Parameters.Add("NOMCLI", OracleDbType.Varchar2, 100).Value = txtNombreCliente.Text;
                cmd.Parameters.Add("MEDIOPAGO", OracleDbType.Int32, 20).Value = cboMedioPago.SelectedValue;
                int n = cmd.ExecuteNonQuery();
                if (n > 0)
                {
                    await this.ShowMessageAsync("VENTA", "Venta realizada como fiado");
                }
                else
                {
                    await this.ShowMessageAsync("VENTA", "Venta no realizada");
                }
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
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
    }
}

