using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaLosYuyitos
{
    public class Detalle_Recepcion
    {
        private int id_recepcion;
        private int codigo_barra;
        private string nombre_producto;
        private int cantidad;
        private int valor;

        public Detalle_Recepcion()
        {

        }

        public Detalle_Recepcion(int recepcion, int cod_barra, string nombre, int canti_producto, int valorC)
        {
            id_recepcion = recepcion;
            codigo_barra = cod_barra;
            nombre_producto = nombre;
            cantidad = canti_producto;
            valor = valorC;

        }

        public int Id_recepcion
        {
            get { return id_recepcion; }
            set { id_recepcion = value; }
        }

        public int Codigo_barra
        {
            get { return codigo_barra; }
            set { codigo_barra = value; }
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

        public int Valor
        {
            get { return valor; }
            set { valor = value; }
        }


    }
}
