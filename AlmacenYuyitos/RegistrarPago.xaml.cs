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
        public string nomUsusario { get; set; }
        List<Detalle_boleta> listDboleta = new List<Detalle_boleta>();
        DateTime fecha = DateTime.Today;
        int monto_total = 0;
        public RegistrarPago()
        {
            this.setConnection();
            InitializeComponent();
            txtFechaVenta.SelectedDate = fecha;
            GenerarNroBoleta();
            txtTotalDescuento.Text = 0.ToString();
            txtTotalVenta.Text = 0.ToString();
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
            GestionarVentas gv = new GestionarVentas();
            gv.Show();
            this.Close();
        }

        private void btnRealizarPago_Click(object sender, RoutedEventArgs e)
        {
            if (checkFiado.IsChecked == true)
            {

            }
            else
            {

            }
        }

        private async void btnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int precio = 0;
                int tipo = 0;
                int total = 0;
                string nombre = null;
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT STOCK, NOMBRE_PRODUCT, PRECIO_VENTA, TIPO_PRODUCTO_ID_TIPPRODUC AS TIPO FROM PRODUCTO WHERE CODIGO_PRODUCTO = :CODIGO";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("CODIGO", OracleDbType.Int32, 40).Value = int.Parse(txtCodigoProducto.Text);
                OracleDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (int.Parse(reader["STOCK"].ToString()) > 0 && int.Parse(reader["STOCK"].ToString()) >= int.Parse(txtCantidad.Text))
                    {
                        nombre = reader["NOMBRE_PRODUCT"].ToString();
                        precio = int.Parse(reader["PRECIO_VENTA"].ToString());
                        total = total + (precio * int.Parse(txtCantidad.Text));
                        tipo = int.Parse(reader["TIPO"].ToString());
                        Detalle_boleta detalle_Boleta = new Detalle_boleta(int.Parse(txtNroBoleta.Text), int.Parse(txtCodigoProducto.Text), nombre, int.Parse(txtCantidad.Text), precio);
                        listDboleta.Add(detalle_Boleta);
                        dgVerProductos.ItemsSource = null;
                        dgVerProductos.ItemsSource = listDboleta;
                        CalcularPromocion(tipo, precio);
                        txtTotalVenta.Text = (int.Parse(txtTotalVenta.Text) + total).ToString();
                        monto_total = monto_total + total;
                    }
                    else
                    {
                        await this.ShowMessageAsync("PRODUCTO", "Producto con stock insuficiente");
                    }
                    
                }
                else
                {
                    await this.ShowMessageAsync("PRODUCTO", "El producto no esta registrado");
                }

                
            }
            catch (Exception)
            {

                await this.ShowMessageAsync("Error", "Ha ocurrido un error"); 
            }
            
        }

        private async void CalcularPromocion(int tipo, int precio)
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
                OracleCommand cmd2 = con.CreateCommand();
                cmd2.CommandText = "SELECT NVL(SUM(DESCUENTO_PORCENTAJE)/100, 0) AS PORCENTAJE, NVL(SUM(DESCUENTO_EFECTIVO), 0) AS EFECTIVO FROM PROMOCION WHERE FECHA_INICIO_PROMO <= :FECHA_INICIO AND FECHA_FIN_PROMO >= :FECHA_FIN AND TIPO_PRODUCTO_ID_TIPPRODUC = :TIPO_PRODUCTO AND CANT_PRODUCTO >= :CANTIDAD AND TIPO_PROMOCION_ID_TIPOPROMO = 2";
                cmd2.CommandType = CommandType.Text;
                cmd2.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                cmd2.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                cmd2.Parameters.Add("TIPO_PRODUCTO", OracleDbType.Int32, 40).Value = tipo;
                cmd2.Parameters.Add("CANTIDAD", OracleDbType.Int32, 40).Value = int.Parse(txtCantidad.Text);
                OracleDataReader reader2 = cmd.ExecuteReader();
                if (reader2.Read())
                {

                    porcentaje = porcentaje + (precio * int.Parse(reader2["PORCENTAJE"].ToString()));
                    efectivo = efectivo + int.Parse(reader2["EFECTIVO"].ToString());
                }
                OracleCommand cmd3 = con.CreateCommand();
                cmd3.CommandText = "SELECT NVL(SUM(DESCUENTO_PORCENTAJE)/100, 0) AS PORCENTAJE, NVL(SUM(DESCUENTO_EFECTIVO), 0) AS EFECTIVO FROM PROMOCION WHERE FECHA_INICIO_PROMO <= :FECHA_INICIO AND FECHA_FIN_PROMO >= :FECHA_FIN AND CANT_PRODUCTO >= :CANTIDAD AND TIPO_PROMOCION_ID_TIPOPROMO = 2";
                cmd3.CommandType = CommandType.Text;
                cmd3.Parameters.Add("FECHA_INICIO", OracleDbType.Date).Value = fecha;
                cmd3.Parameters.Add("FECHA_FIN", OracleDbType.Date).Value = fecha;
                cmd3.Parameters.Add("CANTIDAD", OracleDbType.Int32, 40).Value = int.Parse(txtCantidad.Text);
                OracleDataReader reader3 = cmd.ExecuteReader();
                if (reader3.Read())
                {

                    porcentaje = porcentaje + (precio * int.Parse(reader3["PORCENTAJE"].ToString()));
                    efectivo = efectivo + int.Parse(reader3["EFECTIVO"].ToString());
                }

                txtTotalDescuento.Text = (porcentaje + efectivo).ToString();
                txtTotalVenta.Text = (int.Parse(txtTotalVenta.Text) + int.Parse(txtTotalDescuento.Text)).ToString();
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
    }
}
