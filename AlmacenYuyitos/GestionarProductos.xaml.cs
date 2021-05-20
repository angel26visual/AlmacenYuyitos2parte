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
    /// Lógica de interacción para GestionarProductos.xaml
    /// </summary>
    public partial class GestionarProductos 
    {
        public GestionarProductos()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();

        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Close();
        }

        private void btnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            AgregarProductos ap = new AgregarProductos();
            ap.Show();
            this.Close();
        }

        private void btnVisualizarProducto_Click(object sender, RoutedEventArgs e)
        {
            VisualizarProductos vp = new VisualizarProductos();
            vp.Show();
            this.Close();
        }
    }
}
