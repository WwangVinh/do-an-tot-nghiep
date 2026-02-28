using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public int? CarId { get; set; }

    public DateTime? OrderDate { get; set; }

    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? ShippingAddress { get; set; }

    public int? PromotionId { get; set; }

    public virtual Car? Car { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual Promotion? Promotion { get; set; }

    public virtual User? User { get; set; }
}
