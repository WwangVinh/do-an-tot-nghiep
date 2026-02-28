using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class CarSpecification
{
    public int SpecId { get; set; }

    public int CarId { get; set; }

    public string Category { get; set; } = null!;

    public string SpecName { get; set; } = null!;

    public string SpecValue { get; set; } = null!;

    public virtual Car Car { get; set; } = null!;
}
