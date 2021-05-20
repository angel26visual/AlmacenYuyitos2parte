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
    /// Lógica de interacción para VisualizarDeudas.xaml
    /// </summary>
    public partial class VisualizarDeudas
    {
        public VisualizarDeudas()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            ModyElimCliente myec = new ModyElimCliente();
            myec.Show();
            this.Close();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnVerInformacion_Click(object sender, RoutedEventArgs e)
        {
            VerInformacion vi = new VerInformacion();
            vi.Show();
            this.Close();
        }
    }
}
