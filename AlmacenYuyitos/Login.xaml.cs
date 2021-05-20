namespace AlmacenYuyitos
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PaginaInicio pgi = new PaginaInicio();
            pgi.Show();
            this.Close();
        }
    }
}
