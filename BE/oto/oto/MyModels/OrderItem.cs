using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace oto.MyModels;

public partial class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    public int? CarId { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }

    [ForeignKey("CarId")]
    [InverseProperty("OrderItems")]
    public virtual Car? Car { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("OrderItems")]
    public virtual Order? Order { get; set; }
}
