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
    /// Lógica de interacción para ControlarRecepciones.xaml
    /// </summary>
    public partial class ControlarRecepciones 
    {
        public ControlarRecepciones()
        {
            InitializeComponent();
        }

        private void btnAtras_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();
            this.Close();
        }

        private void btnIngresarRecepcion_Click(object sender, RoutedEventArgs e)
        {
            IngresarRecepcion ir = new IngresarRecepcion();
            ir.Show();
            this.Close();
        }
    }
}
