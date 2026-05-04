using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEntities.Models;

[Table("CarInventories")]
public class CarInventory
{
    [Key]
    public int InventoryId { get; set; }

    [Required]
    public int ShowroomId { get; set; }

    [Required]
    public int CarId { get; set; }

    [Required]
    public int Quantity { get; set; } = 0;

    [Required]
    [MaxLength(50)]
    public string DisplayStatus { get; set; } = "OnDisplay";

    // Màu xe cụ thể của lô hàng này — nullable vì kho cũ chưa có màu
    [MaxLength(100)]
    public string? Color { get; set; }

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ShowroomId")]
    public virtual Showroom? Showroom { get; set; }

    [ForeignKey("CarId")]
    public virtual Car? Car { get; set; }
}