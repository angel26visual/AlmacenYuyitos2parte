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
    /// Lógica de interacción para PaginaInicio.xaml
    /// </summary>
    public partial class PaginaInicio 
    {
        public PaginaInicio()
        {
            InitializeComponent();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnMenuAdministrador_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

        private void btnMenuVendedor_Click(object sender, RoutedEventArgs e)
        {
            MenuVendedor mv = new MenuVendedor();
            mv.Show();
            this.Close();
        }
    }
}
