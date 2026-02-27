using System;
using System.Collections.Generic;

namespace OtoBackend.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    public int? CarId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Car? Car { get; set; }

    public virtual Order? Order { get; set; }
}
