using System;
using System.Collections.Generic;

namespace PizzeriaCC.Models;

public partial class PedidoDetalle
{
    public int Id { get; set; }

    public int PedidoId { get; set; }

    public int PizzaId { get; set; }

    public int Cantidad { get; set; }

    public decimal Subtotal { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;

    public virtual Pizza Pizza { get; set; } = null!;
}
