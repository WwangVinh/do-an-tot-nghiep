using System;
using System.Collections.Generic;

namespace CarSales.API.Models;

public partial class CarModel
{
    public int ModelId { get; set; }

    public int BrandId { get; set; }

    public string ModelName { get; set; } = null!;

    public string? BodyType { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
}
