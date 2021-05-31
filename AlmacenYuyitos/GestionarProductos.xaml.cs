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
using Microsoft.Win32;
using System.IO;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para GestionarProductos.xaml
    /// </summary>
    public partial class GestionarProductos
    {
        OracleConnection con = null;
        string ruta_imagen = null;
        public GestionarProductos()
        {
            this.setConnection();
            InitializeComponent();
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

        private void ActualizarDataGrid()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT CODIGO_PRODUCTO , NOMBRE_PRODUCT , PRECIO_COMPRA , PRECIO_VENTA, " +
                    "STOCK , STOCK_CRITICO ,FECH_ELABO_PRODUCT , FECH_VENCI_PRODUCT, MARCA, COD_BARRA_PRODUCT , IMG_PRODUC ,TIPO_PRODUCTO_ID_TIPPRODUC,  PROVEEDOR_RUT_PROVEE " +
                    "FROM PRODUCTO ";
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgProductos.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }
        }

        private void CargaCboTipoDeProducto()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_TIPPRODUC , DESCRIP_TIPPRODUC FROM TIPO_PRODUCTO";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboTipoDeProducto.ItemsSource = dt.AsDataView();
                cboTipoDeProducto.DisplayMemberPath = "DESCRIP_TIPPRODUC";
                cboTipoDeProducto.SelectedValuePath = "ID_TIPPRODUC";
            }
            catch (Exception)
            {


            }
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();

        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Close();
        }




        private async void btnRegistrarProducto_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
                FileStream fs = new FileStream(ruta_imagen, FileMode.Open, FileAccess.Read);
                byte[] blob = new byte[fs.Length];
                fs.Read(blob, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.BindByName = true;
                OracleParameter blobParameter = new OracleParameter();
                blobParameter.OracleDbType = OracleDbType.Blob;
                blobParameter.ParameterName = "FOTO";
                blobParameter.Value = blob;

                cmd.Parameters.Add("CODIGO_PRODUCTO", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtCodigoProducto.Text);

            if (txtNombreDeProducto.Text.Replace(" ", string.Empty).Length >= 3)
            {
                cmd.Parameters.Add("NOMBRE_PRODUCT", OracleDbType.Varchar2, 100).Value = txtNombreDeProducto.Text;


            }
            else
            {
                await this.ShowMessageAsync("Error", "el nombre del producto debe tener mas de 3 caracteres!");
                btnRegistrarProducto.IsEnabled = true;
                return;

            }
            cmd.Parameters.Add("PRECIO_COMPRA", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtPrecioDeCompra.Text);
                cmd.Parameters.Add("PRECIO_VENTA", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtPrecioDeVenta.Text);
                cmd.Parameters.Add("STOCK", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtStock.Text);
                cmd.Parameters.Add("STOCK_CRITICO", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtStockCritico.Text);
                cmd.Parameters.Add("FECH_ELABO_PRODUCT", OracleDbType.Date).Value = dpFechaElaboracion.SelectedDate;
                cmd.Parameters.Add("FECH_VENCI_PRODUCT", OracleDbType.Date).Value = dpFechaDeVencimiento.SelectedDate;

            if (txtMarcaProducto.Text.Replace(" ", string.Empty).Length >= 3)
            {
                cmd.Parameters.Add("MARCA", OracleDbType.Varchar2, 100).Value = txtMarcaProducto.Text;
            }
            else
            {
                await this.ShowMessageAsync("Error", "La Marca del producto debe tener mas de 3 caracteres!");
                btnRegistrarProducto.IsEnabled = true;
                return;

            }


                cmd.Parameters.Add("PROVEEDOR_RUT_PROVEE", OracleDbType.Varchar2, 100).Value = txtRutProveedor.Text;
                cmd.Parameters.Add("COD_BARRA_PRODUCT", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtCodigoBarraProducto.Text);
                cmd.Parameters.Add(blobParameter);
                cmd.Parameters.Add("TIPO_PRODUCTO_ID_TIPPRODUC", OracleDbType.Int32, 20).Value = cboTipoDeProducto.SelectedValue;
                cmd.CommandText = "INSERT INTO PRODUCTO(CODIGO_PRODUCTO , NOMBRE_PRODUCT , PRECIO_COMPRA , PRECIO_VENTA , STOCK, STOCK_CRITICO , FECH_ELABO_PRODUCT , FECH_VENCI_PRODUCT, MARCA, PROVEEDOR_RUT_PROVEE, COD_BARRA_PRODUCT , IMG_PRODUC, TIPO_PRODUCTO_ID_TIPPRODUC) VALUES(:CODIGO_PRODUCTO , :NOMBRE_PRODUCT , :PRECIO_COMPRA , :PRECIO_VENTA , :STOCK, :STOCK_CRITICO , :FECH_ELABO_PRODUCT , :FECH_VENCI_PRODUCT, :MARCA, :PROVEEDOR_RUT_PROVEE, :COD_BARRA_PRODUCT , :FOTO, :TIPO_PRODUCTO_ID_TIPPRODUC)";
                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("Agregado", "Producto se agregó correctamente");
                        this.ActualizarDataGrid();
                        resetAll();
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }


            //}
            //catch (Exception ex)
            //{

              //  await this.ShowMessageAsync("Error", e.ToString());
            //}
        }



        private void resetAll()
        {
            txtCodigoProducto.Text = "";
            txtNombreDeProducto.Text = "";
            txtPrecioDeCompra.Text = "";
            txtPrecioDeVenta.Text = "";
            txtStock.Text = "";
            txtStockCritico.Text = "";
            txtCodigoBarraProducto.Text = "";
            txtMarcaProducto.Text = "";
            txtRutProveedor.Text = "";


            btnRegistrarProducto.IsEnabled = true;
            btnActualizarProducto.IsEnabled = false;
            btnEliminarProducto.IsEnabled = false;
        }

        private async void btnActualizarProducto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileStream fs = new FileStream(ruta_imagen, FileMode.Open, FileAccess.Read);
                byte[] blob = new byte[fs.Length];
                fs.Read(blob, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.BindByName = true;
                OracleParameter blobParameter = new OracleParameter();
                blobParameter.OracleDbType = OracleDbType.Blob;
                blobParameter.ParameterName = "FOTO";
                blobParameter.Value = blob;

                cmd.Parameters.Add("CODIGO_PRODUCTO", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtCodigoProducto.Text);

                if (txtNombreDeProducto.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("NOMBRE_PRODUCT", OracleDbType.Varchar2, 100).Value = txtNombreDeProducto.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "el nombre del producto debe tener mas de 3 caracteres!");
                    btnRegistrarProducto.IsEnabled = true;
                    return;

                }

                cmd.Parameters.Add("PRECIO_COMPRA", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtPrecioDeCompra.Text);
                cmd.Parameters.Add("PRECIO_VENTA", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtPrecioDeVenta.Text);
                cmd.Parameters.Add("STOCK", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtStock.Text);
                cmd.Parameters.Add("STOCK_CRITICO", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtStockCritico.Text);
                cmd.Parameters.Add("FECH_ELABO_PRODUCT", OracleDbType.Date).Value = dpFechaElaboracion.SelectedDate;
                cmd.Parameters.Add("FECH_VENCI_PRODUCT", OracleDbType.Date).Value = dpFechaDeVencimiento.SelectedDate;
                cmd.Parameters.Add("MARCA", OracleDbType.Varchar2, 100).Value = txtMarcaProducto.Text;
                cmd.Parameters.Add("PROVEEDOR_RUT_PROVEE", OracleDbType.Varchar2, 100).Value = txtRutProveedor.Text;
                cmd.Parameters.Add("COD_BARRA_PRODUCT", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtCodigoBarraProducto.Text);
                cmd.Parameters.Add(blobParameter);
                cmd.Parameters.Add("TIPO_PRODUCTO_ID_TIPPRODUC", OracleDbType.Int32, 20).Value = cboTipoDeProducto.SelectedValue;

                cmd.CommandText = "UPDATE PRODUCTO SET NOMBRE_PRODUCT=:NOMBRE_PRODUCT , PRECIO_COMPRA=:PRECIO_COMPRA , PRECIO_VENTA=:PRECIO_VENTA , STOCK=:STOCK , STOCK_CRITICO=:STOCK_CRITICO , FECH_ELABO_PRODUCT=:FECH_ELABO_PRODUCT , FECH_VENCI_PRODUCT=:FECH_VENCI_PRODUCT , MARCA=:MARCA , PROVEEDOR_RUT_PROVEE=:PROVEEDOR_RUT_PROVEE , COD_BARRA_PRODUCT=:COD_BARRA_PRODUCT, IMG_PRODUC=:FOTO , TIPO_PRODUCTO_ID_TIPPRODUC=:TIPO_PRODUCTO_ID_TIPPRODUC WHERE CODIGO_PRODUCTO=:CODIGO_PRODUCTO";
                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n >= 0)
                    {
                        await this.ShowMessageAsync("ACTUALIZADO", "Producto Actualizado correctamente");
                        this.ActualizarDataGrid();
                        resetAll();
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }

            }



            catch (Exception ex)
            {

                await this.ShowMessageAsync("Error", e.ToString());
            }

        }



        private async void btnEliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("CODIGO_PRODUCTO", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtCodigoProducto.Text);
                cmd.CommandText = "delete from producto where codigo_producto = :CODIGO_PRODUCTO";
                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("eliminado", "Producto eliminado correctamente");
                        this.ActualizarDataGrid();
                        this.resetAll();

                    }
                }

                catch (Exception)
                {
                    await this.ShowMessageAsync("Error", "no se pudo eliminar");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void btnLimpiarCampos_Click(object sender, RoutedEventArgs e)
        {
            this.resetAll();
        }

        private void cboTipoDeProducto_Loaded(object sender, RoutedEventArgs e)
        {
            CargaCboTipoDeProducto();
        }

        private void dgProductos_Loaded(object sender, RoutedEventArgs e)
        {
            this.ActualizarDataGrid();
        }

        private void dgProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                txtCodigoProducto.Text = dr["CODIGO_PRODUCTO"].ToString();
                txtNombreDeProducto.Text = dr["NOMBRE_PRODUCT"].ToString();
                txtPrecioDeCompra.Text = dr["PRECIO_COMPRA"].ToString();
                txtPrecioDeVenta.Text = dr["PRECIO_VENTA"].ToString();
                txtStock.Text = dr["STOCK"].ToString();
                txtStockCritico.Text = dr["STOCK_CRITICO"].ToString();
                dpFechaElaboracion.Text = dr["FECH_ELABO_PRODUCT"].ToString();
                dpFechaDeVencimiento.Text = dr["FECH_VENCI_PRODUCT"].ToString();
                txtMarcaProducto.Text = dr["MARCA"].ToString();
                txtRutProveedor.Text = dr["PROVEEDOR_RUT_PROVEE"].ToString();
                txtCodigoBarraProducto.Text = dr["COD_BARRA_PRODUCT"].ToString();
                cboTipoDeProducto.SelectedValue = dr["TIPO_PRODUCTO_ID_TIPPRODUC"].ToString();

                btnRegistrarProducto.IsEnabled = false;
                btnActualizarProducto.IsEnabled = true;
                btnEliminarProducto.IsEnabled = true;

            }
        }

        private void btnVolver_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void btnCargarImagen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Archivos de imágen (.jpg)|*.jpg|All Files (*.*)|*.*";

            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = true;
            bool? respuesta = openFileDialog1.ShowDialog();
            ruta_imagen = openFileDialog1.FileName;
            imgFoto.Source = new BitmapImage(new Uri(openFileDialog1.FileName));

        }

        private async void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            MessageDialogResult respuesta= await this.ShowMessageAsync("Imprimir", "¿Desea Imprimir el código del producto?", MessageDialogStyle.AffirmativeAndNegative);
            if (respuesta == MessageDialogResult.Affirmative)
            {
                if(dialog.ShowDialog() == true)
                {
           

                    String texto = txtCodigoBarraProducto.Text;
                    string nombre = txtNombreDeProducto.Text;
                    Run r = new Run(texto);
                    Run a = new Run(nombre);
                    string union = nombre + "  " + texto;
                    Paragraph parrafo = new Paragraph();
                    parrafo.Inlines.Add(a);
                    
                    parrafo.Inlines.Add(r);
                    FlowDocument doc = new FlowDocument(parrafo);
                    doc.PagePadding = new Thickness(100);
                    dialog.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, union);
                    
        
                    return;
                }
            }
            else if(respuesta == MessageDialogResult.Negative){
                return;
            }
        }
    }
 }

