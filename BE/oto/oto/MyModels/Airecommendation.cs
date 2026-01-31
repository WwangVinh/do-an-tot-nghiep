using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace oto.MyModels;

[Table("AIRecommendations")]
public partial class Airecommendation
{
    [Key]
    public int RecommendationId { get; set; }

    public int? UserId { get; set; }

    public int? CarId { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CarId")]
    [InverseProperty("Airecommendations")]
    public virtual Car? Car { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Airecommendations")]
    public virtual User? User { get; set; }
}
