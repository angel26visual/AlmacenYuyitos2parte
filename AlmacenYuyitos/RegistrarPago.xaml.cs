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
    /// Lógica de interacción para RegistrarPago.xaml
    /// </summary>
    public partial class RegistrarPago 
    {
        public RegistrarPago()
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
            GestionarVentas gv = new GestionarVentas();
            gv.Show();
            this.Close();
        }

        private void btnRealizarPago_Click(object sender, RoutedEventArgs e)
        {
            RealizarPagoVenta rpv = new RealizarPagoVenta();
            rpv.Show();
            this.Close();
        }
    }
}
