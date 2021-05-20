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
    /// Lógica de interacción para GestionarDelivery.xaml
    /// </summary>
    public partial class GestionarDelivery 
    {
       
        public GestionarDelivery()
        {
            InitializeComponent();
        }

        private void btnVolverMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnRegistrarDelivey_Click(object sender, RoutedEventArgs e)
        {
            RegistrarPedidoDelivery rpd = new RegistrarPedidoDelivery();
            rpd.Show();
            
        }

        private void btnVerPedidosDelivery_Click(object sender, RoutedEventArgs e)
        {
            VisualizarPedidoDelivery vpd = new VisualizarPedidoDelivery();
            vpd.Show();
           
        }
    }
}
