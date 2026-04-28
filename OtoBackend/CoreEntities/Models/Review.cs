using System;
using System.Collections.Generic;
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
    public string Phone { get; set; } = string.Empty; // Dùng SĐT làm định danh

    public string? OrderCode { get; set; } // Mã để xác thực giao dịch

    public int Rating { get; set; } // Số sao

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsApproved { get; set; } = false; // Mặc định là đợi duyệt

    [ForeignKey("CarId")]
    public virtual Car? Car { get; set; }
}