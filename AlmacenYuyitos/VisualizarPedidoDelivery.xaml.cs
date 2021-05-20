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
       

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
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
