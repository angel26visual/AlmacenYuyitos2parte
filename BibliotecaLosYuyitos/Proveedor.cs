using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotecaLosYuyitos
{
    public class Proveedor
    {
        private string rutProveedor;
        private string nombreProveedor;
        private string direccionProveedor;
        private int telefono1_proveedor;
        private int telefono2_proveedor;
        private string nombreServidor;
        private int telefonoServidor;

        public string RutProveedor { get => rutProveedor; set => rutProveedor = value; }
        public string NombreProveedor { get => nombreProveedor; set => nombreProveedor = value; }
        public string DireccionProveedor { get => direccionProveedor; set => direccionProveedor = value; }
        public int Telefono1_proveedor { get => telefono1_proveedor; set => telefono1_proveedor = value; }
        public int Telefono2_proveedor { get => telefono2_proveedor; set => telefono2_proveedor = value; }
        public string NombreServidor { get => nombreServidor; set => nombreServidor = value; }
        public int TelefonoServidor { get => telefonoServidor; set => telefonoServidor = value; }

        public Proveedor()
        {
            this.Init();
        }

        private void Init()
        {
            RutProveedor = string.Empty;
            NombreProveedor = string.Empty;
            DireccionProveedor = string.Empty;
            Telefono1_proveedor = 0;
            Telefono2_proveedor = 0;
            NombreServidor = string.Empty;
            TelefonoServidor = 0;
        }
    }
}
