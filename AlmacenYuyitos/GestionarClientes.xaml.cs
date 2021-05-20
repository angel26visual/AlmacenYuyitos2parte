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
    /// Lógica de interacción para GestionarClientes.xaml
    /// </summary>
    public partial class GestionarClientes 
    {
        public GestionarClientes()
        {
            InitializeComponent();
        }

        private void btnVolverAlMenu_Click(object sender, RoutedEventArgs e)
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

        private void btnAgregarClientes_Click(object sender, RoutedEventArgs e)
        {
            AgregarClientes ac = new AgregarClientes();
            ac.Show();
            this.Close();
        }

        private void btnVisualzarClientes_Click(object sender, RoutedEventArgs e)
        {
            VisualizarCliente vc = new VisualizarCliente();
            vc.Show();
            this.Close();


        }
    }
}
