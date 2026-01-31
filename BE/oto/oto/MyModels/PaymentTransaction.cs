using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace oto.MyModels;

public partial class PaymentTransaction
{
    [Key]
    public int TransactionId { get; set; }

    public int? OrderId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Amount { get; set; }

    [StringLength(50)]
    public string? PaymentMethod { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TransactionDate { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("PaymentTransactions")]
    public virtual Order? Order { get; set; }
}
