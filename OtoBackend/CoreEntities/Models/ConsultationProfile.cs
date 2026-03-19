using CoreEntities.Models;
using System;
using System;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreEntities.Models;

[Table("ConsultationProfiles")]
public class ConsultationProfile
{
    [Key]
    public int ProfileId { get; set; }

    public int? UserId { get; set; }
    public int? SessionId { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? BudgetMin { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? BudgetMax { get; set; }

    [MaxLength(100)]
    public string? PreferredBodyStyle { get; set; }

    [MaxLength(100)]
    public string? PreferredBrand { get; set; }

    [MaxLength(255)]
    public string? Purpose { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }

    [ForeignKey("SessionId")]
    public virtual ChatSession? ChatSession { get; set; }
}