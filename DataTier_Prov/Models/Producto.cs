using System;
using System.Collections.Generic;

namespace DataTier_Prov.Models;

public partial class Producto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Cantidad { get; set; }

    public string? Correo { get; set; }


}
