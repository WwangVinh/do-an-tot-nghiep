using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string? Code { get; set; }

    public decimal? DiscountPercentage { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
