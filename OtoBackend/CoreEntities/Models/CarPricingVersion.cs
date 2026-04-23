using System;

namespace CoreEntities.Models;

public partial class CarPricingVersion
{
    public int PricingVersionId { get; set; }

    public int CarId { get; set; }

    public string VersionName { get; set; } = null!;

    public decimal PriceVnd { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Car Car { get; set; } = null!;
}
