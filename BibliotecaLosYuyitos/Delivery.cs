using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaLosYuyitos
{
    public class Delivery 
    {
        private Categorias categoria;
        private Productos producto;
        private int cantidad;
        private string nombreCliente;
        private int telefonoContacto;
        private string direccion;
        private int valorDespacho;
        private DateTime fechaEntrega;
        private int total;

        public Categorias Categoria { get => categoria; set => categoria = value; }
        public Productos Producto { get => producto; set => producto = value; }
        public int Cantidad { get => cantidad; set => cantidad = value; }
        public string NombreCliente { get => nombreCliente; set => nombreCliente = value; }
        public int TelefonoContacto { get => telefonoContacto; set => telefonoContacto = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public int ValorDespacho { get => valorDespacho; set => valorDespacho = value; }
        public DateTime FechaEntrega { get => fechaEntrega; set => fechaEntrega = value; }
        public int Total { get => total; set => total = value; }

        //creamos enumeradores para llenar categoria
        public enum Categorias { Categoria1, Categoria2, Categoria3 }
        public enum Productos { Producto1, Producto2, Producto3 }

        public Delivery()
        {
            this.Init();
        }

        private void Init()
        {
            Categoria = Categorias.Categoria1;
            Producto = 0;
            Cantidad = 0;
            NombreCliente = string.Empty;
            TelefonoContacto = 0;
            Direccion = string.Empty;
            ValorDespacho = 0;
            FechaEntrega = (DateTime)DateTime.Now;
            Total = 0;

        }
    }
}
