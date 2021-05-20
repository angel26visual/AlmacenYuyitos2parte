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
    /// Lógica de interacción para ConsultarPromociones.xaml
    /// </summary>
    public partial class ConsultarPromociones 
    {
        public ConsultarPromociones()
        {
            InitializeComponent();
        }

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            GestionarVentas gv = new GestionarVentas();
            gv.Show();
            this.Close();

        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();

        }
    }
}
