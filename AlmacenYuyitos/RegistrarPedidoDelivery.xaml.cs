using BibliotecaLosYuyitos;
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
using static BibliotecaLosYuyitos.Delivery;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para RegistrarPedidoDelivery.xaml
    /// </summary>
    public partial class RegistrarPedidoDelivery 
    {
        List<Delivery> listasDelivery = new List<Delivery>();
        public RegistrarPedidoDelivery()
        {
            InitializeComponent();
            cboCategoria.ItemsSource = Enum.GetValues(typeof(Categorias));
            cboCategoria.SelectedIndex=0;

            cboProducto.ItemsSource = Enum.GetValues(typeof(Productos));
            cboProducto.SelectedIndex = 0;
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }

        private void btnGuardarPedidoDelivery_Click(object sender, RoutedEventArgs e)
        {
            
            ActualizarDatos();
        }

        private void ActualizarDatos()
        {
            dgDelivery.ItemsSource = listasDelivery;
            dgDelivery.Items.Refresh();
        }

       
    }
}
