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

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            PaginaInicio ini = new PaginaInicio();
            ini.Show();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login inicio = new Login();
            inicio.Show();
            this.Close();
        }

        private void btnGestionarUsuarios_Click(object sender, RoutedEventArgs e)
        {
            GestionarUsuarios gu = new GestionarUsuarios();
            gu.Show();
            gu.nomUsuario = nomUsuario;
            this.Close();
            
        }

        private void btnGestionarProductos_Click(object sender, RoutedEventArgs e)
        {
            GestionarProductos gp = new GestionarProductos();
            gp.Show();
            this.Close();
        }

        private void btnGestionarProveedor_Click(object sender, RoutedEventArgs e)
        {
            GestionarProveedor gp = new GestionarProveedor();
            gp.Show();
            this.Close();

        }

        private void btnGestionarClientes_Click(object sender, RoutedEventArgs e)
        {
            GestionarClientes gc = new GestionarClientes();
            gc.Show();
            this.Close();
        }

        private void btnGestionarVentas_Click(object sender, RoutedEventArgs e)
        {
            GestionarVentas gv = new GestionarVentas();
            gv.Show();
            this.Close();
        }

        private void btnOrdenPedidos_Click(object sender, RoutedEventArgs e)
        {
            MenuOrdenDePedidos mop = new MenuOrdenDePedidos();
            mop.Show();
            this.Close();
        }

        private void btnGestionarDelivery_Click(object sender, RoutedEventArgs e)
        {
            GestionarDelivery gd = new GestionarDelivery();
            gd.Show();
            this.Close();
        }

        private void btnControlarRecepciones_Click(object sender, RoutedEventArgs e)
        {
            ControlarRecepciones cr = new ControlarRecepciones();
            cr.Show();
            this.Close();
        }

        private void btnGestionPromociones_Click(object sender, RoutedEventArgs e)
        {
            GestionarPromociones gp = new GestionarPromociones();
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
    }
}
