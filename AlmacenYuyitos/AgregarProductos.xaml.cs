using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para AgregarProductos.xaml
    /// </summary>
    public partial class AgregarProductos
    {
        public AgregarProductos()
        {
            InitializeComponent();
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            GestionarProductos gp = new GestionarProductos();
            gp.Show();
            this.Close();
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Login log = new Login();
            log.Show();
            this.Close();
        }

        private void btnCargarImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de Imagen (.jpg)|*.jpg|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.Multiselect = false;

          
            BitmapImage foto = new BitmapImage();
            foto.BeginInit();
            foto.UriSource = new Uri(ofd.FileName);
            foto.EndInit();
            foto.Freeze();
            imgFoto.Source = foto;
                    


              
               
        }
    }
}
