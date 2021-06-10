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
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using MahApps.Metro.Controls.Dialogs;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para PagarDeuda.xaml
    /// </summary>
    public partial class PagarDeuda 
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        public PagarDeuda(string usuario)
        {
            this.setConnection();
            InitializeComponent();
            nomUsuario = usuario;
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

        private void btnBuscar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();

                cmd.CommandText = "select b.nro_boleta boleta, b.fecha_venta, b.monto_total, v.descrip_tipventa tipo_venta, d.descripcion_estadeuda estado_deuda, b.medio_pago_id_mediopago medio_pago from boleta b join tipo_venta v on v.id_tipventa=b.tipo_venta_id_tipventa join estado_deuda d on d.id_estadeuda=b.estado_deuda_id_estadeuda where d.descripcion_estadeuda = 'No pagada' and b.cliente_rut_cli = :CLIENTE";
                cmd.Parameters.Add("CLIENTE", OracleDbType.Varchar2, 100).Value = txtRut.Text;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                dgVerDeuda.ItemsSource = dt.DefaultView;
                dr.Close();


            }
            catch (Exception)
            {
            }
        }

        private void btnvolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarPagos pagos = new GestionarPagos(nomUsuario);
            this.Close();
            pagos.Show();
        }

        private void dgVerDeuda_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnPagar.IsEnabled = true;
        }

        private async void btnPagar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageDialogResult result = await this.ShowMessageAsync("Pagar", "¿Desea Pagar deuda?", MessageDialogStyle.AffirmativeAndNegative);

                if (result == MessageDialogResult.Affirmative)
                {
                    DataRowView datos = dgVerDeuda.SelectedItem as DataRowView;
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT INTO PAGO(ID_PAGO , FECH_PAGO, MONTO_PAGO , MEDIO_PAGO_ID_MEDIOPAGO, BOLETA_NRO_BOLETA ) " +
                                   "VALUES(:ID_PAGO, :FECH_PAGO, :MONTO, :MEDIO_PAGO, :BOLETA )";
                    cmd.Parameters.Add("ID_PAGO", OracleDbType.Int32, 100).Value = int.Parse(datos["boleta"].ToString());
                    cmd.Parameters.Add("FECH_PAGO", OracleDbType.Date, 100).Value = DateTime.Now;
                    cmd.Parameters.Add("MONTO", OracleDbType.Int32, 100).Value = int.Parse(datos["monto_total"].ToString());
                    cmd.Parameters.Add("MEDIO_PAGO", OracleDbType.Int32, 100).Value = int.Parse(datos["medio_pago"].ToString());
                    cmd.Parameters.Add("BOLETA", OracleDbType.Int32, 100).Value = int.Parse(datos["boleta"].ToString());

                    OracleCommand cmd2 = con.CreateCommand();
                    cmd2.CommandText = "update Boleta set ESTADO_DEUDA_ID_ESTADEUDA =2 where NRO_BOLETA=:BOLETA";
                    cmd2.Parameters.Add("BOLETA", OracleDbType.Int32, 100).Value = int.Parse(datos["boleta"].ToString());
                    try
                    {
                        int n = cmd.ExecuteNonQuery();
                        int m = cmd2.ExecuteNonQuery();
                        if (n > 0 && m > 0)
                        {
                            await this.ShowMessageAsync("pagada", "boleta N°" + datos["boleta"] + " pagada correctamente");

                            dgVerDeuda.ItemsSource = null;
                            dgVerDeuda.Items.Refresh();
                            btnPagar.IsEnabled = false;

                        }
                        else
                        {
                            await this.ShowMessageAsync("no pagada", "boleta N°" + datos["boleta"] + " no se pudo pagar");

                        }

                    }
                    catch (Exception ex)
                    {
                        await this.ShowMessageAsync("Error", ex.ToString());
                    }



                }
                else
                {
                    await this.ShowMessageAsync("Cancelado", "El pago no fue realizado!");

                }

            }
            catch (Exception)
            {

                await this.ShowMessageAsync("error", "El pago no fue realizado!");
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
    }
    }

