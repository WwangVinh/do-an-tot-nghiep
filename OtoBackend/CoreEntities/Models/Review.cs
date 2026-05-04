using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEntities.Models;

public class Review
{
    [Key]
    public int ReviewId { get; set; }

    public int CarId { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    public string? OrderCode { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsApproved { get; set; } = false;

    // ✅ Thêm UserId nullable để EF không tự tạo shadow property gây lỗi
    public int? UserId { get; set; }

    [ForeignKey("CarId")]
    public virtual Car? Car { get; set; }
}