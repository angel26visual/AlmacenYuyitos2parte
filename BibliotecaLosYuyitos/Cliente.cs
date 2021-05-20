using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaLosYuyitos
{
    public class Cliente
    {
        private string rut_cliente;
        private string nombre_cliente;
        private string apellido_cliente;
        private int fono_cliente;
        private string correo_cliente;

        public string Rut_cliente { get => rut_cliente; set => rut_cliente = value; }
        public string Nombre_cliente { get => nombre_cliente; set => nombre_cliente = value; }
        public string Apellido_cliente { get => apellido_cliente; set => apellido_cliente = value; }
        public int Fono_cliente { get => fono_cliente; set => fono_cliente = value; }
        public string Correo_cliente { get => correo_cliente; set => correo_cliente = value; }

        public Cliente()
        {
            this.Init();
        }

        private void Init()
        {
            rut_cliente = string.Empty;
            nombre_cliente = string.Empty;
            apellido_cliente = string.Empty;
            fono_cliente = 0;
            correo_cliente = string.Empty;
        }
    }
}
