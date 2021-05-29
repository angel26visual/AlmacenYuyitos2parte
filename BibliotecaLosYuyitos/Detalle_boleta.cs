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
        private int cantidad;

        public Detalle_boleta()
        {
                
        }

        public Detalle_boleta (int boleta, int producto, int canti_producto)
        {
            nro_boleta = boleta;
            codigo_producto = producto;
            cantidad = canti_producto;
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

        public int Cantidad
        {
            get { return cantidad; }
            set { cantidad = value; }
        }
    }
}
