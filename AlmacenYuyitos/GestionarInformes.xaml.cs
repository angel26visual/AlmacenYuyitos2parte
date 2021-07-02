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

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para GestionarInformes.xaml
    /// </summary>
    public partial class GestionarInformes
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        DateTime fecha = DateTime.Today;
        int clientesR = 0;
        int clientesDI = 0;
        int clientesSD = 0;
        int clientesDP = 0;
        int usuariosR = 0;
        int usuariosA = 0;
        int usuariosV = 0;
        int orden = 0;
        int promocion = 0;
        int promocionV = 0;
        int promocionP = 0;
        int promocionF = 0;
        int proveedor = 0;
        int delivery = 0;
        int deliveryP = 0;
        int deliveryE = 0;
        int deliveryET = 0;
        int ventas = 0;
        int ventasN = 0;
        int ventasF = 0;
        int ventasFP = 0;
        int ventasFI = 0;
        int productos = 0;
        int productosSC = 0;
        int recepcion = 0;
        int pagos = 0;
        public GestionarInformes(string usuario)
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

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
            this.Close();
        }

        private void btnTodos_Click(object sender, RoutedEventArgs e)
        {
            checkClientes.IsChecked = true;
            checkUsuarios.IsChecked = true;
            checkOrdenPedidos.IsChecked = true;
            checkGestionVentas.IsChecked = true;
            checkGestionProductos.IsChecked = true;
            checkPagos.IsChecked = true;
            checkPromociones.IsChecked = true;
            checkProveedores.IsChecked = true;
            checkDelivery.IsChecked = true;
            checkRecepcion.IsChecked = true;
        }

        private void btnNinguno_Click(object sender, RoutedEventArgs e)
        {
            checkClientes.IsChecked = false;
            checkUsuarios.IsChecked = false;
            checkOrdenPedidos.IsChecked = false;
            checkGestionVentas.IsChecked = false;
            checkGestionProductos.IsChecked = false;
            checkPagos.IsChecked = false;
            checkPromociones.IsChecked = false;
            checkProveedores.IsChecked = false;
            checkDelivery.IsChecked = false;
            checkRecepcion.IsChecked = false;
        }

        private async void btnGenerarInforme_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkClientes.IsChecked == true)
                {
                    ComprobarClientesR();
                    ComprobarClienesDI();
                    ComrpobarClientesSD();
                    ComprobarClientesDP();
                }
                if (checkUsuarios.IsChecked == true)
                {
                    ComprobarUsuariosR();
                    ComprobarUsuariosA();
                    ComprobarUsuariosV();
                }
                if (checkOrdenPedidos.IsChecked == true)
                {
                    ComprobarOrdenes();
                }
                if (checkPromociones.IsChecked == true)
                {
                    ComprobarPromociones();
                    ComprobarPromocionesV();
                    ComprobarPromocionesP();
                    ComprobarPromocionesF();
                }
                if (checkProveedores.IsChecked == true)
                {
                    ComprobarProveedores();
                }
                if (checkDelivery.IsChecked == true)
                {
                    ComprobarDelivery();
                    ComprobarDeliveryP();
                    ComprobarDeliveryE();
                    ComprobarDeliveryET();
                }
                if (checkGestionVentas.IsChecked == true)
                {
                    ComprobarVentas();
                    ComprobarVentasN();
                    ComprobarVentasF();
                    ComprobarVentasFP();
                    ComprobarVentasFI();
                }
                if (checkGestionProductos.IsChecked == true)
                {
                    ComprobarProductos();
                    ComprobarProductosSC();
                }
                if (checkRecepcion.IsChecked == true)
                {
                    ComprobarRecepcion();
                }
                if (checkPagos.IsChecked == true)
                {
                    ComprobarPagos();
                }

                VistaInforme informe = new VistaInforme(nomUsuario);
                informe.lbClientesR.Content = clientesR;
                informe.lbClientesD.Content = clientesDI;
                informe.lbClientesSD.Content = clientesSD;
                informe.lbClientesDP.Content = clientesDP;
                informe.lbUserR.Content = usuariosR;
                informe.lbUserA.Content = usuariosA;
                informe.lbUserV.Content = usuariosV;
                informe.lbOrdenR.Content = orden;
                informe.lbPromocionesR.Content = promocion;
                informe.lbPromocionesV.Content = promocionV;
                informe.lbPromocionesP.Content = promocionP;
                informe.lbPromocionesF.Content = promocionF;
                informe.lbProveedorR.Content = proveedor;
                informe.lbDeliveryR.Content = delivery;
                informe.lbDeliveryP.Content = deliveryP;
                informe.lbDeliveryV.Content = deliveryE;
                informe.lbDeliveryET.Content = deliveryET;
                informe.lbVentasR.Content = ventas;
                informe.lbVentasN.Content = ventasN;
                informe.lbVentasF.Content = ventasF;
                informe.lbVentasFP.Content = ventasFP;
                informe.lbVentasFI.Content = ventasFI;
                informe.lbProductoR.Content = productos;
                informe.lbProductoSC.Content = productosSC;
                informe.lbRecepcionesR.Content = recepcion;
                informe.lbPagosR.Content = pagos;
                informe.Show();
                this.Close();
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarClientesR()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS CLIENTES FROM CLIENTE";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    clientesR = int.Parse(reader["CLIENTES"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("CLIENTES", "No se ha podido obtener la información de los clientes");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarClienesDI()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(DISTINCT(CLIENTE_RUT_CLI)),0) AS IMPAGAS FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 2 AND ESTADO_DEUDA_ID_ESTADEUDA = 1";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    clientesDI = int.Parse(reader["IMPAGAS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("CLIENTES", "No se ha podido obtener la información de los clientes");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComrpobarClientesSD()
        {
            try
            {

            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarClientesDP()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(DISTINCT(CLIENTE_RUT_CLI)),0) AS PAGADAS FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 2 AND ESTADO_DEUDA_ID_ESTADEUDA = 2";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    clientesDP = int.Parse(reader["PAGADAS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("CLIENTES", "No se ha podido obtener la información de los clientes");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarUsuariosR()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS USUARIOS FROM TRABAJADOR";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    usuariosR = int.Parse(reader["USUARIOS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("USUARIOS", "No se ha podido obtener la información de los usuarios");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarUsuariosA()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS USUARIOS FROM TRABAJADOR WHERE CARGO_TRABAJADOR_ID_CARGO = 1";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    usuariosA = int.Parse(reader["USUARIOS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("USUARIOS", "No se ha podido obtener la información de los usuarios");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarUsuariosV()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS USUARIOS FROM TRABAJADOR WHERE CARGO_TRABAJADOR_ID_CARGO = 2";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    usuariosV = int.Parse(reader["USUARIOS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("USUARIOS", "No se ha podido obtener la información de los usuarios");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarOrdenes()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS ORDEN FROM ORDEN_PED";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    orden = int.Parse(reader["ORDEN"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("ORDENES DE PEDIDOS", "No se ha podido obtener la información de las ordenes de pedidos");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarPromociones()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS PROMOCION FROM PROMOCION";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    promocion = int.Parse(reader["PROMOCION"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("PROMOCIONES", "No se ha podido obtener la información de las promociones");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarPromocionesV()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS PROMOCION FROM PROMOCION WHERE FECHA_INICIO_PROMO <= :FECHA_INICIO AND FECHA_FIN_PROMO >= :FECHA_FIN";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                cmd.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    promocionV = int.Parse(reader["PROMOCION"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("PROMOCIONES", "No se ha podido obtener la información de las promociones");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarPromocionesP()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS PROMOCION FROM PROMOCION WHERE FECHA_FIN_PROMO < :FECHA_FIN";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    promocionP = int.Parse(reader["PROMOCION"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("PROMOCIONES", "No se ha podido obtener la información de las promociones");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarPromocionesF()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS PROMOCION FROM PROMOCION WHERE FECHA_INICIO_PROMO > :FECHA_INICIO";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    promocionF = int.Parse(reader["PROMOCION"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("PROMOCIONES", "No se ha podido obtener la información de las promociones");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarProveedores()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS PROVEEDOR FROM PROVEEDOR";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    proveedor = int.Parse(reader["PROVEEDOR"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("PROVEEDORES", "No se ha podido obtener la información de los proveedores");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarDelivery()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS DELIVERY FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 3";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    delivery = int.Parse(reader["DELIVERY"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("DELIVERY", "No se ha podido obtener la información de los delivery");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarDeliveryP()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS DELIVERY FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 3 AND ESTADO_PED_ID_ESTADELIVERY = 1";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    deliveryP = int.Parse(reader["DELIVERY"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("DELIVERY", "No se ha podido obtener la información de los delivery");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarDeliveryE()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS DELIVERY FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 3 AND ESTADO_PED_ID_ESTADELIVERY = 2";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    deliveryE = int.Parse(reader["DELIVERY"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("DELIVERY", "No se ha podido obtener la información de los delivery");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarDeliveryET()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS DELIVERY FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 3 AND ESTADO_PED_ID_ESTADELIVERY = 3";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    deliveryET = int.Parse(reader["DELIVERY"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("DELIVERY", "No se ha podido obtener la información de los delivery");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarVentas()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS VENTAS FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 1 OR TIPO_VENTA_ID_TIPVENTA = 2";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ventas = int.Parse(reader["VENTAS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("VENTAS", "No se ha podido obtener la información de las ventas");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarVentasN()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS VENTAS FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 1";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ventasN = int.Parse(reader["VENTAS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("VENTAS", "No se ha podido obtener la información de las ventas");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarVentasF()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS VENTAS FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 2";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ventasF = int.Parse(reader["VENTAS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("VENTAS", "No se ha podido obtener la información de las ventas");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarVentasFP()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS VENTAS FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 2 AND ESTADO_DEUDA_ID_ESTADEUDA = 2";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ventasFP = int.Parse(reader["VENTAS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("VENTAS", "No se ha podido obtener la información de las ventas");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarVentasFI()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS VENTAS FROM BOLETA WHERE TIPO_VENTA_ID_TIPVENTA = 2 AND ESTADO_DEUDA_ID_ESTADEUDA = 1";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ventasFI = int.Parse(reader["VENTAS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("VENTAS", "No se ha podido obtener la información de las ventas");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarProductos()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS PRODUCTOS FROM PRODUCTO";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    productos = int.Parse(reader["PRODUCTOS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("PRODUCTOS", "No se ha podido obtener la información de los productos");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarProductosSC()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS PRODUCTOS FROM PRODUCTO WHERE STOCK < STOCK_CRITICO";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    productosSC = int.Parse(reader["PRODUCTOS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("PRODUCTOS", "No se ha podido obtener la información de los productos");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarRecepcion()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS RECEPCION FROM CONTROL_RECEP";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    recepcion = int.Parse(reader["RECEPCION"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("RECEPCIÓN", "No se ha podido obtener la información de las recepciones");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }

        private async void ComprobarPagos()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT NVL(COUNT(*),0) AS PAGOS FROM PAGO";
                cmd.CommandType = CommandType.Text;
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    pagos = int.Parse(reader["PAGOS"].ToString());
                }
                else
                {
                    await this.ShowMessageAsync("PAGOS", "No se ha podido obtener la información de los pagos");
                }
            }
            catch (Exception)
            {
                await this.ShowMessageAsync("Error", "Ha ocurrido un error");
            }
        }
    }
}
