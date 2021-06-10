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
using MahApps.Metro.Controls.Dialogs;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Configuration;
using MahApps.Metro.Controls.Dialogs;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para GestionarPagos.xaml
    /// </summary>
    public partial class GestionarPagos
    {
        OracleConnection con = null;
        string nomUsuario = string.Empty;
        public GestionarPagos(string usuario)
        {
            InitializeComponent();
            this.setConnection();
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


        private void btnVerDeudas_Click(object sender, RoutedEventArgs e)
        {
            VerPagos vp = new VerPagos(nomUsuario);
            vp.Show();
            this.Close();

        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void btnPagarDeudas_Click(object sender, RoutedEventArgs e)
        {
            PagarDeuda pd = new PagarDeuda(nomUsuario);
            pd.Show();
            this.Close();
        }

        private void btnVerCuenta_Click(object sender, RoutedEventArgs e)
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

        private void btnCuenta_Click(object sender, RoutedEventArgs e)
        {
            Cuenta cuenta = new Cuenta(nomUsuario);
            cuenta.Show();
            this.Close();
        }
    }
}
