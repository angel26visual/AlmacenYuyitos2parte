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
    /// Lógica de interacción para VerInformacion.xaml
    /// </summary>
    public partial class VerInformacion
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        int cargo = 0;
        string nombre = string.Empty;
        string apellido = string.Empty;
        public VerInformacion(string usuario)
        {
            InitializeComponent();
            setConnection();
            ActualizarEstado();
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
        private void ActualizarEstado()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_ESTADEUDA , DESCRIPCION_ESTADEUDA FROM ESTADO_DEUDA";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboEstado.ItemsSource = dt.AsDataView();
                cboEstado.DisplayMemberPath = "DESCRIPCION_ESTADEUDA";
                cboEstado.SelectedValuePath = "ID_ESTADEUDA";

            }
            catch (Exception e)
            {
            }

        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            VisualizarDeudas vd = new VisualizarDeudas(txtRutlienteDeuda.Text, nomUsuario);
            vd.Show();
            this.Close();

        }

        private async void btnModificarDeuda_Click(object sender, RoutedEventArgs e)
        {
            
          
                /*
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                if (dpFechaDeuda.SelectedDate != null)
                { cmd.Parameters.Add("FECHA_DEUDA", OracleDbType.Date).Value = dpFechaDeuda.SelectedDate; }
                else { await this.ShowMessageAsync("Error", "la fecha debe ser valida!"); }
                if (txtMontoDeuda.Text.Replace(" ", string.Empty) != null || int.Parse(txtMontoDeuda.Text) < 0)
                { cmd.Parameters.Add("MONTO", OracleDbType.Int32, 100).Value = txtMontoDeuda.Text; }
                else { await this.ShowMessageAsync("Error", "debe ingresar un monto!"); }
                if (cboEstado.SelectedValue != null)
                { cmd.Parameters.Add("ESTADO_ID", OracleDbType.Int32, 20).Value = cboEstado.SelectedValue; }
                else { await this.ShowMessageAsync("Error", "debe seleccionar un estado!"); }
                cmd.Parameters.Add("id", OracleDbType.Varchar2, 100).Value = txtDeudaId.Text;
                cmd.CommandText = "update deuda set fech_deuda =:FECHA_DEUDA, monto_deuda=:MONTO,estado_deuda_id_estadeuda=:ESTADO_ID where id_deuda=:id";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("actualizado", "deuda actualizado correctamente");
                        VisualizarDeudas v = new VisualizarDeudas(txtRutlienteDeuda.Text, nomUsuario);
                        this.Close();
                        v.Show();

                    }
                }
                catch (Exception)
                {
                    await this.ShowMessageAsync("Error", "no se pudo actualizar");
                }
            }
            catch (Exception)
            {

                throw;
            }*/
            }

        private async void btnEliminarDeuda_Click(object sender, RoutedEventArgs e)
            {
                /*
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("id", OracleDbType.Int32, 100).Value = int.Parse(txtDeudaId.Text);
                cmd.CommandText = "delete from deuda where id_deuda = :id";

                try
                {
                    int n = cmd.ExecuteNonQuery();
                    if (n > 0)
                    {
                        await this.ShowMessageAsync("eliminado", "deuda eliminada correctamente");
                        VisualizarDeudas v = new VisualizarDeudas(txtRutlienteDeuda.Text, nomUsuario);
                        this.Close();
                        v.Show();

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
            } */
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

