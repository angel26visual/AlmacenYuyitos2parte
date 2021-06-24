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
    /// Lógica de interacción para GestionarPromociones.xaml
    /// </summary>
    public partial class GestionarPromociones
    {
        string ruta_imagen = null;
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        public GestionarPromociones(string usuario)
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

        private void ActualizarDataGrid()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_PROMOCION , FECHA_INICIO_PROMO , FECHA_FIN_PROMO , DESCRIP_PROMO ,CANT_PRODUCTO , DESCUENTO_PORCENTAJE , DESCUENTO_EFECTIVO , TIPO_PRODUCTO_ID_TIPPRODUC , TIPO_PROMOCION_ID_TIPOPROMO FROM PROMOCION order by ID_PROMOCION ASC";
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgPromociones.ItemsSource = dt.DefaultView;
                dr.Close();

            }
            catch (Exception e)
            { }
        }

        private void cboTipoDeProducto_Loaded(object sender, RoutedEventArgs e)
        {
            this.CargarCboTipoDeProducto();
        }


        private void CargarCboTipoDeProducto()
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

                throw;
            }
        }

        private void CargarCboTipoDePromocion()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_TIPOPROMO , DESCRIP_TIPOPROMO FROM TIPO_PROMOCION";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboTipoDePromocion.ItemsSource = dt.AsDataView();
                cboTipoDePromocion.DisplayMemberPath = "DESCRIP_TIPOPROMO";
                cboTipoDePromocion.SelectedValuePath = "ID_TIPOPROMO";
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void cboTipoDePromocion_Loaded(object sender, RoutedEventArgs e)
        {
            this.CargarCboTipoDePromocion();
        }

        private void btnVolverAlMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
            this.Close();
        }

        private async void btnAgregarPromocion_Click(object sender, RoutedEventArgs e)
        {

                FileStream fs = new FileStream(ruta_imagen, FileMode.Open, FileAccess.Read);
                 byte[] blob = new byte[fs.Length];
                 fs.Read(blob, 0, Convert.ToInt32(fs.Length));
                 fs.Close();
                 OracleCommand cmd = new OracleCommand("sp_insertar_promocion", con);
                 cmd.CommandType = CommandType.StoredProcedure;
                 cmd.BindByName = true;
                 OracleParameter blobParameter = new OracleParameter();
                 blobParameter.OracleDbType = OracleDbType.Blob;
                 blobParameter.ParameterName = "FOTO";
                 blobParameter.Value = blob;

                cmd.Parameters.Add("id", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdPromocion.Text);
                cmd.Parameters.Add(blobParameter);
                cmd.Parameters.Add("inicio", OracleDbType.Date).Value = dpFechaDeInicio.SelectedDate;
                cmd.Parameters.Add("fin", OracleDbType.Date).Value = dpFechaTermino.SelectedDate;
                cmd.Parameters.Add("descripcion", OracleDbType.Varchar2, 100).Value = txtDescripcion.Text;
                cmd.Parameters.Add("cantidad", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtCantidadDeProductos.Text);
                cmd.Parameters.Add("porcentaje", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtPorcentajeDescuento.Text);
                cmd.Parameters.Add("efectivo", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtDescEfectivo.Text);
                cmd.Parameters.Add("tipo_producto", OracleDbType.Int32, 20).Value = cboTipoDeProducto.SelectedValue;
                cmd.Parameters.Add("tipo_promocion", OracleDbType.Int32, 20).Value = cboTipoDePromocion.SelectedValue;

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n < 0)
                    {
                        await this.ShowMessageAsync("Agregada", "Promocion se agregó correctamente");
                        Limpiar();
                        this.ActualizarDataGrid();
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }
            
          

        }
        public void Limpiar()
        {
            txtDescripcion.Text = "";
            txtCantidadDeProductos.Text = "";
            txtPorcentajeDescuento.Text = "";
            txtDescEfectivo.Text = "";
            txtIdPromocion .Text= "";
            imgFoto.Source = null;
        }

        private void btnLimpiarCampos_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }

        private async void btnModificarPromocion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileStream fs = new FileStream(ruta_imagen, FileMode.Open, FileAccess.Read);
                byte[] blob = new byte[fs.Length];
                fs.Read(blob, 0, Convert.ToInt32(fs.Length));
                fs.Close();
                OracleCommand cmd = new OracleCommand("sp_actualizar_promociones", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                OracleParameter blobParameter = new OracleParameter();
                blobParameter.OracleDbType = OracleDbType.Blob;
                blobParameter.ParameterName = "FOTO";
                blobParameter.Value = blob;



                cmd.Parameters.Add("fecha_inicio", OracleDbType.Date).Value = dpFechaDeInicio.SelectedDate;
                cmd.Parameters.Add(blobParameter);
                cmd.Parameters.Add("fecha_fin", OracleDbType.Date).Value = dpFechaTermino.SelectedDate;
                if (txtDescripcion.Text.Replace(" ", string.Empty).Length >= 3)
                {
                    cmd.Parameters.Add("descripcion", OracleDbType.Varchar2, 100).Value = txtDescripcion.Text;
                }
                else
                {
                    await this.ShowMessageAsync("Error", "La descripción de la promoción debe tener mas de 3 caracteres!");
                    btnAgregarPromocion.IsEnabled = true;
                    return;

                }
                    cmd.Parameters.Add("cantidad", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtCantidadDeProductos.Text);
                cmd.Parameters.Add("porcentaje", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtPorcentajeDescuento.Text);
                cmd.Parameters.Add("efectivo", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtDescEfectivo.Text);
                cmd.Parameters.Add("tipo_product", OracleDbType.Int32, 20).Value = cboTipoDeProducto.SelectedValue;
                cmd.Parameters.Add("tipo_promo", OracleDbType.Int32, 20).Value = cboTipoDePromocion.SelectedValue;
                cmd.Parameters.Add("id", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdPromocion.Text);
                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n < 0)
                    {
                        await this.ShowMessageAsync("Modificada", "Promocion se modificó correctamente");
                        this.ActualizarDataGrid();
                        Limpiar();
                    }
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Error", ex.ToString());
                }
            }
            catch (Exception ex)
            {

                await this.ShowMessageAsync("Error", ex.ToString());
            }
        }

        private void dgPromociones_Loaded(object sender, RoutedEventArgs e)
        {
            ActualizarDataGrid();
        }

       

        private void dgPromociones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dg = sender as DataGrid;
            DataRowView dr = dg.SelectedItem as DataRowView;
            if (dr != null)
            {
                txtIdPromocion.Text = dr["ID_PROMOCION"].ToString();
                dpFechaDeInicio.Text = dr["FECHA_INICIO_PROMO"].ToString();
                dpFechaTermino.Text = dr["FECHA_FIN_PROMO"].ToString();
                txtDescripcion.Text = dr["DESCRIP_PROMO"].ToString();
                txtCantidadDeProductos.Text = dr["CANT_PRODUCTO"].ToString();
                txtPorcentajeDescuento.Text = dr["DESCUENTO_PORCENTAJE"].ToString();
                txtDescEfectivo.Text = dr["DESCUENTO_EFECTIVO"].ToString();
                cboTipoDeProducto.SelectedValue = dr["TIPO_PRODUCTO_ID_TIPPRODUC"].ToString();
                cboTipoDePromocion.SelectedValue = dr["TIPO_PROMOCION_ID_TIPOPROMO"].ToString();


                btnAgregarPromocion.IsEnabled = false;
                btnModificarPromocion.IsEnabled = true;
                btnEliminarPromocion.IsEnabled = true;
            }
        }

        private async void btnEliminarPromocion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = new OracleCommand("sp_eliminar_promocion", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("id", OracleDbType.Int32, 10).Value = Convert.ToInt32(txtIdPromocion.Text);

                MessageDialogResult respuesta = await this.ShowMessageAsync("ELIMINAR", "¿Desea Eliminar Información de la Promoción Seleccionada?", MessageDialogStyle.AffirmativeAndNegative);
                if (respuesta == MessageDialogResult.Affirmative)
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n < 0)
                    {
                        await this.ShowMessageAsync("eliminada", "Oferta eliminada correctamente");
                        this.ActualizarDataGrid();
                        Limpiar();

                    }
                }
                else
                {
                    return;
                }

               
            }
            catch (Exception)
            {
                throw;
            }
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

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
            this.Close();
        }

        private void btnVolver_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            mw.nomUsuario = nomUsuario;
            mw.btnCuenta.Content = "Bienvenido/a " + nombre + " " + apellido;
            this.Close();
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

        private async void txtIdPromocion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El ID de la promoción debe contener sólo números");
            }
        }

        private async void txtCantidadDeProductos_KeyDown(object sender, KeyEventArgs e)
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

        private async  void txtPorcentajeDescuento_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Porcentaje de Descuento debe contener sólo números");
            }
        }

        private async void txtDescEfectivo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;

                await this.ShowMessageAsync("Error", "El Descuento Efectivo debe contener sólo números");
            }
        }
    }
}

