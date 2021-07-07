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
    /// Lógica de interacción para VerVenta.xaml
    /// </summary>
    public partial class VerVenta 
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        List<Detalle_boleta> listDboleta = new List<Detalle_boleta>();
        DateTime fecha = DateTime.Today;
        public int monto_total = 0;
        int monto_descuento = 0;
        string rutTrab = string.Empty;
        bool validaciones = true;
        public int verifiCli = 0;
        int boleta = 0;
        public VerVenta(string usuario, int boletaV)
        {
            this.setConnection();
            InitializeComponent();
            nomUsuario = usuario;
            DatosUsuarios();
            verifiCli = 0;
            boleta = boletaV;
            CargarLista(boleta);
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
            VisualizarVentas visv = new VisualizarVentas(nomUsuario);
            visv.Show();
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

        private async void txtCodigoProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Código del producto debe contener sólo números");
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

                await this.ShowMessageAsync("Error", "La cantidad de Productos debe contener sólo números");
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

                await this.ShowMessageAsync("Error", "El Monto pagado debe contener sólo números");
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
                                    txtTotalVenta.Text = monto_total.ToString();
                                    dgVerProductos.ItemsSource = null;
                                    dgVerProductos.ItemsSource = listDboleta;
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
                                Detalle_boleta detalle_Boleta = new Detalle_boleta(int.Parse(txtNroBoleta.Text), int.Parse(txtCodigoProducto.Text), nombre, int.Parse(txtCantidad.Text), precio, tipo);
                                listDboleta.Add(detalle_Boleta);
                                dgVerProductos.ItemsSource = null;
                                dgVerProductos.ItemsSource = listDboleta;
                                cantidadNueva = int.Parse(txtCantidad.Text);
                                CalcularPromocionTipo(tipo, precio);
                                CalcularPromocionCantidad(tipo, precio, cantidadNueva);
                                monto_total = monto_total + total;
                                txtTotalVenta.Text = monto_total.ToString();
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
                txtTotalDescuento.Text = monto_descuento.ToString();
                monto_total = monto_total - (porcentaje + efectivo);
                txtTotalVenta.Text = monto_total.ToString();

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
                txtTotalDescuento.Text = monto_descuento.ToString();
                monto_total = monto_total - (porcentaje + efectivo);
                txtTotalVenta.Text = monto_total.ToString();

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
                txtTotalDescuento.Text = monto_descuento.ToString();
                monto_total = monto_total + descuento;
                txtTotalVenta.Text = monto_total.ToString();
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

        private async void btnEiminarProducto_Click(object sender, RoutedEventArgs e)
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
                        dgVerProductos.ItemsSource = null;
                        dgVerProductos.ItemsSource = listDboleta;
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
                                    cantidadVieja = detalle_boleta.Cantidad;
                                    cantidadNueva = int.Parse(txtCantidad.Text);
                                    cantidad = cantidadNueva - cantidadVieja;
                                    detalle_boleta.Cantidad = cantidad;
                                    total = total + (precio * cantidad);
                                    monto_total = monto_total + total;
                                    txtTotalVenta.Text = monto_total.ToString();
                                    dgVerProductos.ItemsSource = null;
                                    dgVerProductos.ItemsSource = listDboleta;
                                    DesCalcularPromocionCantidad(tipo, precio, cantidadVieja);
                                    CalcularPromocionCantidad(tipo, precio, cantidadNueva);
                                    txtCantidad.Text = 0.ToString();
                                    txtCodigoProducto.Text = 0.ToString();
                                }
                                else
                                {
                                    await this.ShowMessageAsync("PRODUCTO", "Producto con stock insuficiente");
                                }

                                detalle_boleta.Cantidad = int.Parse(txtCantidad.Text);
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

        private async void btnVerificarCli_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NOMBRE_CLI FROM CLIENTE WHERE RUT_CLI = :RUT";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("RUT", OracleDbType.Varchar2, 40).Value = txtRutCli.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    verifiCli = 1;
                    txtNombreCliente.Text = reader["NOMBRE_CLI"].ToString();
                }
                else
                {
                    verifiCli = 0;
                    await this.ShowMessageAsync("CLIENTE", "Cliente no encontrado");
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
                txtTotalDescuento.Text = monto_descuento.ToString();
                monto_total = monto_total + descuento;
                txtTotalVenta.Text = monto_total.ToString();

            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }

        private async void ValidarN()
        {
            try
            {
                int validar = 0;
                if (listDboleta.Count() <= 0)
                {
                    validar = 1;
                    await this.ShowMessageAsync("VENTA", "No hay productos agregados a la lista");
                }
                if (Convert.ToInt32(txtPago.Text) < monto_total && validar == 0)
                {
                    validar = 1;
                    await this.ShowMessageAsync("VENTA", "El monto de pago es menor al monto total de la venta");
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

        private async void ValidarF()
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
                if (verifiCli == 0 && validar == 0)
                {
                    validar = 1;
                    await this.ShowMessageAsync("VENTA", "No se ha verificado al cliente");
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

        private async void btnModificarVenta_Click(object sender, RoutedEventArgs e)
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
                    ValidarN();
                    if (validaciones)
                    {
                        OracleCommand cmd = new OracleCommand("sp_actualizar_ventaN", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("boleta", OracleDbType.Int32, 20).Value = int.Parse(txtNroBoleta.Text);
                        cmd.Parameters.Add("venta", OracleDbType.Int32, 30).Value = int.Parse(txtTotalVenta.Text) + int.Parse(txtTotalDescuento.Text);
                        cmd.Parameters.Add("descuento", OracleDbType.Int32, 30).Value = int.Parse(txtTotalDescuento.Text);
                        cmd.Parameters.Add("monto", OracleDbType.Int32, 30).Value = monto_total;
                        cmd.Parameters.Add("pago", OracleDbType.Int32, 20).Value = int.Parse(txtPago.Text);
                        cmd.Parameters.Add("rut", OracleDbType.Varchar2, 20).Value = rutTrab;
                        cmd.Parameters.Add("medio_pago", OracleDbType.Int32, 20).Value = cboMedioPago.SelectedValue;
                        int n = cmd.ExecuteNonQuery();
                        if (n < 0)
                        {
                            EliminarDetalle(Convert.ToInt32(txtNroBoleta.Text));
                            foreach (var detalle_Boleta in listDboleta)
                            {
                                GuardarDetalle(detalle_Boleta.Nro_boleta, detalle_Boleta.Codigo_producto, detalle_Boleta.Cantidad);
                            }
                            int vuelto = int.Parse(txtPago.Text) - monto_total;
                            if (vuelto > 0)
                            {
                                await this.ShowMessageAsync("VENTA MODIFICADA", "El vuelto del cliente es:" + " $ " + vuelto + " " + "pesos");
                            }
                            else
                            {
                                await this.ShowMessageAsync("VENTA MODIFICADA", "venta sin vuelto al cliente");
                            }
                        }
                        else
                        {
                            await this.ShowMessageAsync("VENTA", "Venta no modificada");
                        }
                    }

                }
                catch (Exception)
                {
                    await this.ShowMessageAsync("Error", "Ha ocurrido un error");
                }
            }
        }

        private async void btnEliminarVenta_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_eliminar_boleta", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("boleta", OracleDbType.Int32, 20).Value = int.Parse(txtNroBoleta.Text);

                MessageDialogResult respuesta = await this.ShowMessageAsync("ELIMINAR", "¿Desea Eliminar Información de la orden Seleccionada?", MessageDialogStyle.AffirmativeAndNegative);

                if (respuesta == MessageDialogResult.Affirmative)
                {
                    int n = cmd.ExecuteNonQuery();

                    if (n < 0)
                    {
                        await this.ShowMessageAsync("Venta", "Venta Eliminada correctamente");
                        VisualizarVentas ventas = new VisualizarVentas(nomUsuario);
                        ventas.Show();
                        this.Close();
                    }
                    else
                    {
                        await this.ShowMessageAsync("Venta", "Venta NO Eliminada");
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

        private async void EliminarDetalle(int boleta)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_eliminar_detalleB", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("boleta", OracleDbType.Int32, 20).Value = boleta;
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void btnModificarFiado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidarF();
                if (validaciones)
                {
                    OracleCommand cmd = new OracleCommand("sp_actualizar_ventaF", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("boleta", OracleDbType.Int32, 20).Value = int.Parse(txtNroBoleta.Text);
                    cmd.Parameters.Add("venta", OracleDbType.Int32, 30).Value = int.Parse(txtTotalVenta.Text) + int.Parse(txtTotalDescuento.Text);
                    cmd.Parameters.Add("descuento", OracleDbType.Int32, 30).Value = int.Parse(txtTotalDescuento.Text);
                    cmd.Parameters.Add("monto", OracleDbType.Int32, 30).Value = monto_total;
                    cmd.Parameters.Add("pago", OracleDbType.Int32, 20).Value = int.Parse(txtPago.Text);
                    cmd.Parameters.Add("rut", OracleDbType.Varchar2, 20).Value = rutTrab;
                    cmd.Parameters.Add("medio_pago", OracleDbType.Int32, 20).Value = cboMedioPago.SelectedValue;
                    cmd.Parameters.Add("rutC", OracleDbType.Varchar2, 40).Value = txtRutCli.Text;
                    cmd.Parameters.Add("cliente", OracleDbType.Varchar2, 100).Value = txtNombreCliente.Text;
                    int n = cmd.ExecuteNonQuery();
                    if (n < 0)
                    {
                        EliminarDetalle(Convert.ToInt32(txtNroBoleta.Text));
                        foreach (var detalle_Boleta in listDboleta)
                        {
                            GuardarDetalle(detalle_Boleta.Nro_boleta, detalle_Boleta.Codigo_producto, detalle_Boleta.Cantidad);
                        }
                        VerificarClienteflyouts.IsOpen = false;
                        await this.ShowMessageAsync("VENTA FIADO", "Venta modificada con éxito");
                    }
                    else
                    {
                        await this.ShowMessageAsync("VENTA FIADO", "Venta no modificada");
                    }
                }

            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void CargarLista(int boleta)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT BOLETA_NRO_BOLETA AS NRO_BOLETA, PRODUCTO_CODIGO_PRODUCTO AS CODIGO_PRODUCTO, p.NOMBRE_PRODUCT AS NOMBRE_PRODUCTO, CANTI_PRODUCTO AS CANTIDAD, p.PRECIO_VENTA AS PRECIO, p.TIPO_PRODUCTO_ID_TIPPRODUC AS TIPO FROM DETALLE_BOLETA INNER JOIN PRODUCTO p ON PRODUCTO_CODIGO_PRODUCTO = CODIGO_PRODUCTO WHERE BOLETA_NRO_BOLETA = :boleta";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("boleta", OracleDbType.Int32, 20).Value = boleta;
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Detalle_boleta detalle = new Detalle_boleta(int.Parse(reader["NRO_BOLETA"].ToString()), int.Parse(reader["CODIGO_PRODUCTO"].ToString()), reader["NOMBRE_PRODUCTO"].ToString(), int.Parse(reader["CANTIDAD"].ToString()), int.Parse(reader["PRECIO"].ToString()), int.Parse(reader["TIPO"].ToString()));
                    listDboleta.Add(detalle);
                    dgVerProductos.ItemsSource = null;
                    dgVerProductos.ItemsSource = listDboleta;

                }

            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }

        }
    }
}
