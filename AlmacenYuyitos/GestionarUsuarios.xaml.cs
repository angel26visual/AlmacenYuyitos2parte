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
    /// Lógica de interacción para GestionarUsuarios.xaml
    /// </summary>
    public partial class GestionarUsuarios 
    {
        public GestionarUsuarios()
        {
            InitializeComponent();
        }

        private void btnAgregarUsuario_Click(object sender, RoutedEventArgs e)
        {
            AgregarUsuario au = new AgregarUsuario();
            au.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VisualizarUsuario vu = new VisualizarUsuario();
            vu.Show();
            this.Close();
            
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
    }
}
