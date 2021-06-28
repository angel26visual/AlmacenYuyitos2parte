using MahApps.Metro.Controls.Dialogs;
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

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string nomUsuario { get; set; }


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

        private void btnGestionarUsuarios_Click(object sender, RoutedEventArgs e)
        {
            GestionarUsuarios gu = new GestionarUsuarios(nomUsuario);
            gu.Show();
            this.Close();
            
        }

        private void btnGestionarProductos_Click(object sender, RoutedEventArgs e)
        {
            GestionarProductos gp = new GestionarProductos(nomUsuario);
            gp.Show();
            this.Close();
        }

        private void btnGestionarProveedor_Click(object sender, RoutedEventArgs e)
        {
            GestionarProveedor gp = new GestionarProveedor(nomUsuario);
            gp.Show();
            this.Close();

        }

        private void btnGestionarClientes_Click(object sender, RoutedEventArgs e)
        {
            GestionarClientes gc = new GestionarClientes(nomUsuario);
            gc.Show();
            this.Close();
        }

        private void btnGestionarVentas_Click(object sender, RoutedEventArgs e)
        {
            GestionarVentas gv = new GestionarVentas(nomUsuario);
            gv.Show();
            this.Close();
        }

        private void btnOrdenPedidos_Click(object sender, RoutedEventArgs e)
        {
            GenerarOrdenDePedidos mop = new GenerarOrdenDePedidos(nomUsuario);
            mop.Show();
            this.Close();
        }

        private void btnGestionarDelivery_Click(object sender, RoutedEventArgs e)
        {
            GestionarDelivery gd = new GestionarDelivery(nomUsuario);
            gd.Show();
            this.Close();
        }

        private void btnControlarRecepciones_Click(object sender, RoutedEventArgs e)
        {
            ControlarRecepciones cr = new ControlarRecepciones(nomUsuario);
            cr.Show();
            this.Close();
        }

        private void btnGestionPromociones_Click(object sender, RoutedEventArgs e)
        {
            GestionarPromociones gp = new GestionarPromociones(nomUsuario);
            gp.Show();
            this.Close();
        }

        private void btnVerCuenta_Click(object sender, RoutedEventArgs e)
        {
            Cuenta cuenta = new Cuenta(nomUsuario);
            cuenta.Show();
            this.Close();
        }

        private void btnCuenta_Click(object sender, RoutedEventArgs e)
        {
            if (cuentaFlyouts.IsOpen == true){
                cuentaFlyouts.IsOpen = false;
            }
            else
            {
                cuentaFlyouts.IsOpen = true;
            }
            
        }

        private void btnGestionarPagos_Click(object sender, RoutedEventArgs e)
        {
            GestionarPagos gestion = new GestionarPagos(nomUsuario);
            gestion.Show();
            this.Close();
        }

        private void btnGenerarInforme_Click(object sender, RoutedEventArgs e)
        {
            GestionarInformes gi = new GestionarInformes(nomUsuario);
            gi.Show();
            this.Close();
        }
    }
}
