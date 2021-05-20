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
    /// Lógica de interacción para VisualizarProductos.xaml
    /// </summary>
    public partial class VisualizarProductos 
    {
        public VisualizarProductos()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarProductos gp = new GestionarProductos();
            gp.Show();
            this.Close();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnVerProducto_Click(object sender, RoutedEventArgs e)
        {
            ModyElimProductos myep = new ModyElimProductos();
            myep.Show();
            this.Close();
        }
    }
}
