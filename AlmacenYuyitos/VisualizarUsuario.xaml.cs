using System;
using System.Data;
using System.Configuration;
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
using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para VisualizarUsuario.xaml
    /// </summary>
    public partial class VisualizarUsuario
    {
        public string nomUsuario { get; set; }
        public int Cargo { get; set; }
        OracleConnection con = null;
        string commandtable = "SELECT T.RUT_TRAB RUT, T.NOMBRE_TRAB NOMBRE, T.APELLIDO_TRAB APELLIDO, T.FECHA_NACIMIENTO FECHA_DE_NACIMIENTO ,E.DESCRIP_ESTAC ESTADO_CIVIL ,C.NOMBRE_CARGO CARGO , T.CORREO,T.NOM_USUARIO USUARIO,T.CONTRASENA_USUARIO CONTRASENA, CARGO_TRABAJADOR_ID_CARGO ID_CARGO, ESTADO_CIVIL_ID_ESTAC ID_ESTADO FROM TRABAJADOR T JOIN CARGO_TRABAJADOR C ON T.CARGO_TRABAJADOR_ID_CARGO=C.ID_CARGO JOIN ESTADO_CIVIL E ON T.ESTADO_CIVIL_ID_ESTAC = E.ID_ESTAC";

        public VisualizarUsuario()
        {
            setConnection();
            InitializeComponent();
            actualizarDataGrid();
            ActualizarCargo();
            DatosUsuarios();
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
                    Cargo = int.Parse(reader["CARGO_TRABAJADOR_ID_CARGO"].ToString());
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
        private void actualizarDataGrid()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = commandtable;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dgUsuarios.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private void ActualizarCargo()
        {
            try
            {
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "SELECT ID_CARGO , NOMBRE_CARGO FROM CARGO_TRABAJADOR";
                cmd.CommandType = System.Data.CommandType.Text;

                OracleDataReader dr = cmd.ExecuteReader();
                cmd.ExecuteNonQuery();
                OracleDataAdapter oda = new OracleDataAdapter(cmd);

                DataTable dt = new DataTable();
                oda.Fill(dt);
                cboCargo.ItemsSource = dt.AsDataView();
                cboCargo.DisplayMemberPath = "NOMBRE_CARGO";
                cboCargo.SelectedValuePath = "ID_CARGO";

            }
            catch (Exception e)
            {
            }

        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarUsuarios gu = new GestionarUsuarios();
            gu.Show();
            gu.nomUsuario = nomUsuario;
            this.Close();
        }

        private void btnVerUsuario_Click(object sender, RoutedEventArgs e)
        {
            ModyElmUsuario mdye = new ModyElmUsuario();
            DataRowView datos = dgUsuarios.SelectedItem as DataRowView;
            if (datos != null)
            {
                mdye.txtRut.Text = datos["RUT"].ToString();
                mdye.txtNombre.Text = datos["NOMBRE"].ToString();
                mdye.txtApellido.Text = datos["APELLIDO"].ToString();
                mdye.dpFechaNacimiento.Text = datos["FECHA_DE_NACIMIENTO"].ToString();
                mdye.txtCorreo.Text = datos["CORREO"].ToString();
                mdye.txtNombreUsuario.Text = datos["USUARIO"].ToString();
                mdye.txtContrasena.Password = datos["CONTRASENA"].ToString();
                mdye.cboCargo.SelectedValue = int.Parse(datos["ID_CARGO"].ToString());
                mdye.cboEstadoCivil.SelectedValue = int.Parse(datos["ID_ESTADO"].ToString());




                mdye.Show();
                mdye.nomUsuario = nomUsuario;
                this.Close();
            }

        }

        private void btnBuscarPorRut_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = commandtable
                + " WHERE RUT_TRAB = :RUT_TRAB";
            cmd.Parameters.Add("RUT_TRAB", OracleDbType.Varchar2, 100).Value = txtFiltrarRut.Text;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dgUsuarios.ItemsSource = dt.DefaultView;
            dr.Close();
        }

        private void btnFiltrarCargo_Click(object sender, RoutedEventArgs e)
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = commandtable
                + " WHERE CARGO_TRABAJADOR_ID_CARGO = :CARGO";
            cmd.Parameters.Add("CARGO", OracleDbType.Int32, 20).Value = cboCargo.SelectedValue;
            cmd.CommandType = CommandType.Text;
            OracleDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dgUsuarios.ItemsSource = dt.DefaultView;
            dr.Close();

        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
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
    }
}