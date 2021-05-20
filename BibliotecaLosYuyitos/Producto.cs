using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaLosYuyitos
{
    public class Producto { 
    
        private int codigo_producto;
        private string nombre_producto;
        private int precio_compra;
        private int precio_venta;
        private int stock;
        private int stock_critico;
        private DateTime fech_elabo_product;
        private DateTime fech_venci_product;
        private int cod_barra_producto;
   

       

        public Producto()
        {
            this.Init();
        }

        public int Codigo_producto { get => codigo_producto; set => codigo_producto = value; }
        public string Nombre_producto { get => nombre_producto; set => nombre_producto = value; }
        public int Precio_compra { get => precio_compra; set => precio_compra = value; }
        public int Precio_venta { get => precio_venta; set => precio_venta = value; }
        public int Stock { get => stock; set => stock = value; }
        public int Stock_critico { get => stock_critico; set => stock_critico = value; }
        public DateTime Fech_elabo_product { get => fech_elabo_product; set => fech_elabo_product = value; }
        public DateTime Fech_venci_product { get => fech_venci_product; set => fech_venci_product = value; }
        public int Cod_barra_producto { get => cod_barra_producto; set => cod_barra_producto = value; }


        private void Init()
        {
            Codigo_producto = 0;
            Nombre_producto = string.Empty;
            Precio_compra = 0;
            Precio_venta = 0;
            Stock = 0;
            Stock_critico = 0;
            Fech_elabo_product = DateTime.Now;
            Fech_venci_product = DateTime.Now;
            Cod_barra_producto = 0;
          
        }
    }
}
