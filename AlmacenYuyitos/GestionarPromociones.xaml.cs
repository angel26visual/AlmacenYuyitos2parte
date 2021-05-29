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

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para GestionarPromociones.xaml
    /// </summary>
    public partial class GestionarPromociones
    {
        OracleConnection con = null;
        public GestionarPromociones()
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
                cmd.CommandText = "SELECT ID_PROMOCION , IMAGEN_PROMO , FECHA_INICIO_PROMO , FECHA_FIN_PROMO , DESCRIP_PROMO ,CANT_PRODUCTO , DESCUENTO_PORCENTAJE , DESCUENTO_EFECTIVO , TIPO_PRODUCTO_ID_TIPPRODUC , TIPO_PROMOCION_ID_TIPOPROMO FROM PROMOCION order by ID_PROMOCION ASC";
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
            this.Close();
        }

        private async void btnAgregarPromocion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.Add("ID_PROMOCION", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdPromocion.Text);

                cmd.Parameters.Add("FECHA_INICIO_PROMO", OracleDbType.Date).Value = dpFechaDeInicio.SelectedDate;
                cmd.Parameters.Add("FECHA_FIN_PROMO", OracleDbType.Date).Value = dpFechaTermino.SelectedDate;
                cmd.Parameters.Add("DESCRIP_PROMO", OracleDbType.Varchar2, 100).Value = txtDescripcion.Text;
                cmd.Parameters.Add("CANT_PRODUCTO", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtCantidadDeProductos.Text);
                cmd.Parameters.Add("DESCUENTO_PORCENTAJE", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtPorcentajeDescuento.Text);
                cmd.Parameters.Add("DESCUENTO_EFECTIVO", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtDescEfectivo.Text);
                cmd.Parameters.Add("TIPO_PRODUCTO_ID_TIPPRODUC", OracleDbType.Int32, 20).Value = cboTipoDeProducto.SelectedValue;
                cmd.Parameters.Add("TIPO_PROMOCION_ID_TIPOPROMO", OracleDbType.Int32, 20).Value = cboTipoDePromocion.SelectedValue;

                cmd.CommandText = "INSERT INTO PROMOCION(ID_PROMOCION, IMAGEN_PROMO , FECHA_INICIO_PROMO , FECHA_FIN_PROMO , DESCRIP_PROMO , CANT_PRODUCTO, DESCUENTO_PORCENTAJE , DESCUENTO_EFECTIVO , TIPO_PRODUCTO_ID_TIPPRODUC , TIPO_PROMOCION_ID_TIPOPROMO)  VALUES" +
                    "(:ID_PROMOCION, EMPTY_BLOB() ,:FECHA_INICIO_PROMO , :FECHA_FIN_PROMO , :DESCRIP_PROMO ,:CANT_PRODUCTO , :DESCUENTO_PORCENTAJE , :DESCUENTO_EFECTIVO , :TIPO_PRODUCTO_ID_TIPPRODUC , :TIPO_PROMOCION_ID_TIPOPROMO)";


                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("Agregada", "Promocion se agregó correctamente");
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
        public void Limpiar()
        {
            txtDescripcion.Text = "";
            txtCantidadDeProductos.Text = "";
            txtPorcentajeDescuento.Text = "";
            txtDescEfectivo.Text = "";
        }

        private void btnLimpiarCampos_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
        }

        private async void btnModificarPromocion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;

                

                cmd.Parameters.Add("FECHA_INICIO_PROMO", OracleDbType.Date).Value = dpFechaDeInicio.SelectedDate;
                cmd.Parameters.Add("FECHA_FIN_PROMO", OracleDbType.Date).Value = dpFechaTermino.SelectedDate;
                cmd.Parameters.Add("DESCRIP_PROMO", OracleDbType.Varchar2, 100).Value = txtDescripcion.Text;
                cmd.Parameters.Add("CANT_PRODUCTO", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtCantidadDeProductos.Text);
                cmd.Parameters.Add("DESCUENTO_PORCENTAJE", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtPorcentajeDescuento.Text);
                cmd.Parameters.Add("DESCUENTO_EFECTIVO", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtDescEfectivo.Text);
                cmd.Parameters.Add("TIPO_PRODUCTO_ID_TIPPRODUC", OracleDbType.Int32, 20).Value = cboTipoDeProducto.SelectedValue;
                cmd.Parameters.Add("TIPO_PROMOCION_ID_TIPOPROMO", OracleDbType.Int32, 20).Value = cboTipoDePromocion.SelectedValue;
                cmd.Parameters.Add("ID_PROMOCION", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdPromocion.Text);

                cmd.CommandText = "UPDATE PROMOCION SET FECHA_INICIO_PROMO=:FECHA_INICIO_PROMO , FECHA_FIN_PROMO=:FECHA_FIN_PROMO , DESCRIP_PROMO=:DESCRIP_PROMO , CANT_PRODUCTO=:CANT_PRODUCTO, DESCUENTO_PORCENTAJE=:DESCUENTO_PORCENTAJE , DESCUENTO_EFECTIVO=:DESCUENTO_EFECTIVO , TIPO_PRODUCTO_ID_TIPPRODUC=:TIPO_PRODUCTO_ID_TIPPRODUC , TIPO_PROMOCION_ID_TIPOPROMO=:TIPO_PROMOCION_ID_TIPOPROMO WHERE ID_PROMOCION =:ID_PROOMOCION";




                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("Modificada", "Promocion se modificó correctamente");
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
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("ID_PROMOCION", OracleDbType.Int32, 20).Value = Convert.ToInt32(txtIdPromocion.Text);
                cmd.CommandText = "delete from promocion where id_promocion = :ID_PROMOCION";
                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("eliminada", "Oferta eliminada correctamente");
                        this.ActualizarDataGrid();
                        Limpiar();

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
    }
}

