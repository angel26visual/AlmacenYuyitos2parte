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
    /// Lógica de interacción para VisualizarUsuario.xaml
    /// </summary>
    public partial class VisualizarUsuario
    {
        public VisualizarUsuario()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarUsuarios gu = new GestionarUsuarios();
            gu.Show();
            this.Close();
        }

        private void btnVerUsuario_Click(object sender, RoutedEventArgs e)
        {
            ModyElmUsuario mdye = new ModyElmUsuario();
            mdye.Show();
            this.Close();
        }
    }
}
