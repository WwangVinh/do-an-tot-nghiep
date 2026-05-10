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

    public int? CarColorId { get; set; }

    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("ShowroomId")]
    public virtual Showroom? Showroom { get; set; }

    [ForeignKey("CarId")]
    public virtual Car? Car { get; set; }

    // ✅ Navigation property nối sang class CarColor    [ForeignKey("CarColorId")]
    public virtual CarColor? CarColor { get; set; }
}