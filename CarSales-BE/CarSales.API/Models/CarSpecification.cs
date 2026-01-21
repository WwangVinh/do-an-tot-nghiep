using System;
using System.Collections.Generic;

namespace CarSales.API.Models;

public partial class CarSpecification
{
    public int Id { get; set; }

    public int CarId { get; set; }

    public int SpecId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Car Car { get; set; } = null!;

    public virtual Specification Spec { get; set; } = null!;
}
