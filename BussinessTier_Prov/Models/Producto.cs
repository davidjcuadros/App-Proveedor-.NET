using System;

namespace BussinessTier_Prov.Models
{
    public class Producto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public int Cantidad { get; set; }

        public string Correo { get; set; } = string.Empty;
    }
}
