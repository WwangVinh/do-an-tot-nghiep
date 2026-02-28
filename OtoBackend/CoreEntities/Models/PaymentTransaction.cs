using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class PaymentTransaction
{
    public int TransactionId { get; set; }

    public int? OrderId { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string? Status { get; set; }

    public virtual Order? Order { get; set; }
}
