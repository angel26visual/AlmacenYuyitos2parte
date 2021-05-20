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
    /// Lógica de interacción para VisualizarCliente.xaml
    /// </summary>
    public partial class VisualizarCliente 
    {
        public VisualizarCliente()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarClientes gc = new GestionarClientes();
            gc.Show();
            this.Close();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();

        }

        private void btnVerCliente_Click(object sender, RoutedEventArgs e)
        {
            ModyElimCliente mec = new ModyElimCliente();
            mec.Show();
            this.Close();
        }
    }
}
