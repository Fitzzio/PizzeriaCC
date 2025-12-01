using System;
using System.Collections.Generic;

namespace PizzeriaCC.Models;

public partial class Motorizado
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Disponible { get; set; }

    public int EntregasRealizadas { get; set; }

    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
