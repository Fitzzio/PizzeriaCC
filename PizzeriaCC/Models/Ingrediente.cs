using System;
using System.Collections.Generic;

namespace PizzeriaCC.Models;

public partial class Ingrediente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal PrecioExtra { get; set; }

    public virtual ICollection<Pizza> IdPizzas { get; set; } = new List<Pizza>();
}
