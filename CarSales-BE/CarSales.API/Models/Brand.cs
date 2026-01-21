using System;
using System.Collections.Generic;

namespace CarSales.API.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string? CountryOrigin { get; set; }

    public string? LogoUrl { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<CarModel> CarModels { get; set; } = new List<CarModel>();
}
