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
    /// Lógica de interacción para ModyElimCliente.xaml
    /// </summary>
    public partial class ModyElimCliente 
    {
        public ModyElimCliente()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            VisualizarCliente vc = new VisualizarCliente();
            vc.Show();
            this.Close();
        }

        private void btnDeudas_Click(object sender, RoutedEventArgs e)
        {
            VisualizarDeudas vd = new VisualizarDeudas();
            vd.Show();
            this.Close();
        }
    }
}
