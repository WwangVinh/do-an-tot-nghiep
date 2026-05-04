using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEntities.Models;

public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int? CarId { get; set; }

    public DateTime? OrderDate { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? ShippingAddress { get; set; }

    public int? PromotionId { get; set; }

    [MaxLength(50)]
    public string? OrderCode { get; set; }

    // --- Thông tin Khách vãng lai ---
    [Required]
    [MaxLength(255)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Email { get; set; }

    public string? CustomerNote { get; set; }
    public string? SecretToken { get; set; }

    // --- Quản lý Nội bộ ---
    public string? AdminNote { get; set; } // Ghi chú của nhân viên
    public DateTime? LastUpdated { get; set; } = DateTime.Now;

    // Cột ní vừa yêu cầu đây:
    public int? StaffId { get; set; } // ID của nhân viên phụ trách đơn này
    public int? ShowroomId { get; set; } // ID showroom tiếp nhận đơn

    // --- Tài chính ---
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Subtotal { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18, 2)")]
    public decimal FinalAmount { get; set; } = 0;

    [Required]
    [MaxLength(50)]
    public string PaymentStatus { get; set; } = "Unpaid";

    // --- Navigation Properties ---
    [ForeignKey("CarId")]
    public virtual Car? Car { get; set; }

    [ForeignKey("PromotionId")]
    public virtual Promotion? Promotion { get; set; }

    // Liên kết tới nhân viên xử lý
    [ForeignKey("StaffId")]
    [InverseProperty("Orders")]
    public virtual User? Staff { get; set; }

    // Liên kết tới showroom
    [ForeignKey("ShowroomId")]
    public virtual Showroom? Showroom { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}