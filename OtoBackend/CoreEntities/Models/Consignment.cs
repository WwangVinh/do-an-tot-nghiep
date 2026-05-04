using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEntities.Models;

[Table("Consignments")]
public class Consignment
{
    [Key]
    public int ConsignmentId { get; set; }

    // ── Thông tin khách vãng lai (bắt buộc) ──────────────────────────
    [Required]
    [MaxLength(255)]
    public string GuestName { get; set; } = null!;

    [Required]
    [MaxLength(20)]
    public string GuestPhone { get; set; } = null!;

    [MaxLength(255)]
    public string? GuestEmail { get; set; }

    // ── Thông tin xe ─────────────────────────────────────────────────
    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = null!;

    [Required]
    public int Year { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Mileage { get; set; }

    [MaxLength(1000)]
    public string? ConditionDescription { get; set; }

    // ── Giá cả ───────────────────────────────────────────────────────
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal ExpectedPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AgreedPrice { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? CommissionRate { get; set; }

    // ── Trạng thái & liên kết ────────────────────────────────────────
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    public int? LinkedCarId { get; set; }

    // ── Thời gian ────────────────────────────────────────────────────
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // ── Navigation properties ─────────────────────────────────────────
    [ForeignKey("LinkedCarId")]
    public virtual Car? LinkedCar { get; set; }
}