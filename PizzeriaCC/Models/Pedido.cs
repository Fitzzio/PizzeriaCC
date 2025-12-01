using System;
using System.Collections.Generic;

namespace PizzeriaCC.Models;

public partial class Pedido
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public DateTime Fecha { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = null!;

    public string MetodoPago { get; set; } = null!;

    public int? MotorizadoId { get; set; }

    public virtual Motorizado? Motorizado { get; set; }

    public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();

    public virtual Usuario Usuario { get; set; } = null!;
}
