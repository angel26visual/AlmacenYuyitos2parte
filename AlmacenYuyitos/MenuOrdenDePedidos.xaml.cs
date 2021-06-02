using MahApps.Metro.Controls.Dialogs;
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
    /// Lógica de interacción para MenuOrdenDePedidos.xaml
    /// </summary>
    public partial class MenuOrdenDePedidos 
    {
        public MenuOrdenDePedidos()
        {
            InitializeComponent();
        }

        private void btnMenuPrincipal_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private async void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MessageDialogResult respuesta = await this.ShowMessageAsync("Cerrar Sesión", "¿Desea cerrar Sesión?", MessageDialogStyle.AffirmativeAndNegative);

            if (respuesta == MessageDialogResult.Affirmative)
            {
                await this.ShowMessageAsync("Éxito", "Usted ha cerrado sesión exitosamente");
                Login log = new Login();
                log.Show();
                this.Close();
            }
            else
            {
                return;
            }
        }

        private void btnGenerarOrdenDePedidos_Click(object sender, RoutedEventArgs e)
        {
            GenerarOrdenDePedidos gop = new GenerarOrdenDePedidos();
            gop.Show();
            this.Close();
        }

        private void btnVisualizarOrdenPedidos_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
