using System;
using System.Collections.Generic;

namespace PizzeriaCC.Models;

public partial class Pizza
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal PrecioBase { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();

    public virtual ICollection<Ingrediente> IdIngredientes { get; set; } = new List<Ingrediente>();
}
