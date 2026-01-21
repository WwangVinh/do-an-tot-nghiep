using System;
using System.Collections.Generic;

namespace CarSales.API.Models;

public partial class Specification
{
    public int SpecId { get; set; }

    public string SpecName { get; set; } = null!;

    public string? Unit { get; set; }

    public string? Category { get; set; }

    public virtual ICollection<CarSpecification> CarSpecifications { get; set; } = new List<CarSpecification>();
}
