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
    /// Lógica de interacción para ConsultarPromociones.xaml
    /// </summary>
    public partial class ConsultarPromociones 
    {
        public ConsultarPromociones()
        {
            InitializeComponent();
            txtInicio.SelectedDate = DateTime.Today;
            txtTermino.SelectedDate = DateTime.Today;
        }

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            GestionarVentas gv = new GestionarVentas();
            gv.Show();
            this.Close();

        }

        private void btnFiltrarInicio_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFiltrarTermino_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
