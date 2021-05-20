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
    /// Lógica de interacción para MenuVendedor.xaml
    /// </summary>
    public partial class MenuVendedor 
    {
        public MenuVendedor()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            PaginaInicio pi = new PaginaInicio();
            pi.Show();
            this.Close();

        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login lg = new Login();
            lg.Show();
            this.Close();
        }
    }
}
