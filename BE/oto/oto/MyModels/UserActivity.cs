using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace oto.MyModels;

[Table("UserActivity")]
public partial class UserActivity
{
    [Key]
    public int ActivityId { get; set; }

    public int? UserId { get; set; }

    [StringLength(50)]
    public string? ActivityType { get; set; }

    public int? CarId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ActivityDate { get; set; }

    [ForeignKey("CarId")]
    [InverseProperty("UserActivities")]
    public virtual Car? Car { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserActivities")]
    public virtual User? User { get; set; }
}
