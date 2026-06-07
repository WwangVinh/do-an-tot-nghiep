using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEntities.Models;

public partial class CarColor
{
    [Key]
    public int CarColorId { get; set; }

    public int CarId { get; set; }

    [Required]
    [StringLength(100)]
    public string ColorName { get; set; } = null!; // Tên màu: Đỏ, Đen...

    [StringLength(20)]
    public string? HexCode { get; set; } // Mã màu để hiện UI: #FF0000

    public string? ImageUrl { get; set; } // Link ảnh xe với màu này

    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    // Khóa ngoại nối về bảng Car
    [ForeignKey("CarId")]
    public virtual Car Car { get; set; } = null!;

    // Một màu cụ thể của xe có thể nằm trong nhiều kho (Showroom) khác nhau
    public virtual ICollection<CarInventory> CarInventories { get; set; } = new List<CarInventory>();
}