using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaLosYuyitos
{
    public class Detalle_boleta
    {
        private int nro_boleta;
        private int codigo_producto;
        private string nombre_producto;
        private int cantidad;
        private int precio;
        private int tipo;

        public Detalle_boleta()
        {
                
        }

        public Detalle_boleta (int boleta, int producto, string nombre, int canti_producto, int precio_venta, int tipo_p)
        {
            nro_boleta = boleta;
            codigo_producto = producto;
            nombre_producto = nombre;
            cantidad = canti_producto;
            precio = precio_venta;
            tipo = tipo_p;
        }

        public int Nro_boleta
        {
            get { return nro_boleta; }
            set { nro_boleta = value; }
        }

        public int Codigo_producto
        {
            get { return codigo_producto; }
            set { codigo_producto = value; }
        }

        public string Nombre_producto
        {
            get { return nombre_producto; }
            set { nombre_producto = value; }
        }

        public int Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }

        public int Precio
        {
            get { return precio; }
            set { precio = value; }
        }

        public int Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
    }
}
