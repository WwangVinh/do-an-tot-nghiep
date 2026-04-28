using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEntities.Models;

public partial class OrderItem
{
    [Key]
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    // Nếu là dòng bán Xe thì điền CarId
    public int? CarId { get; set; }

    // Nếu là dòng bán Phụ kiện thì điền AccessoryId
    public int? AccessoryId { get; set; }

    [MaxLength(50)]
    public string? ItemType { get; set; } // "Car" hoặc "Accessory"

    public int? Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }

    public virtual Car? Car { get; set; }
    public virtual Order? Order { get; set; }

    // Liên kết tới bảng phụ kiện mới tạo
    [ForeignKey("AccessoryId")]
    public virtual Accessory? Accessory { get; set; }
}