using BibliotecaLosYuyitos;
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
using System.Data;
using MahApps.Metro.Controls.Dialogs;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para VisualizarPedidoDelivery.xaml
    /// </summary>
    public partial class VisualizarPedidoDelivery 
    {
       
        public VisualizarPedidoDelivery()
        {
          
            InitializeComponent();
     
        
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

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
           
            this.Close();
        }

        private void btnVerInfoDelivery_Click(object sender, RoutedEventArgs e)
        {
            VerInformacionDelivery vid = new VerInformacionDelivery();
            vid.Show();
            this.Close();
        }

        

       
    }
}
