using CoreEntities.Models;
using System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreEntities.Models;

[Table("Consignments")]
public class Consignment
{
    [Key]
    public int ConsignmentId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Brand { get; set; }

    [Required]
    [MaxLength(100)]
    public string Model { get; set; }

    [Required]
    public int Year { get; set; }

    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Mileage { get; set; }

    [MaxLength(1000)]
    public string? ConditionDescription { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? ExpectedPrice { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AgreedPrice { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal? CommissionRate { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    public int? LinkedCarId { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [ForeignKey("LinkedCarId")]
    public virtual Car? LinkedCar { get; set; }
}