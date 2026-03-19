using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreEntities.Models;

public partial class Airecommendation
{
    public int RecommendationId { get; set; }

    public int? UserId { get; set; }

    public int? CarId { get; set; }

    public string? Reason { get; set; }

    public bool? IsHelpful { get; set; }

    [MaxLength(500)]
    public string? FeedbackNote { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Car? Car { get; set; }

    public virtual User? User { get; set; }
}
