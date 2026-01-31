using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace oto.MyModels;

public partial class CarImage
{
    [Key]
    public int CarImageId { get; set; }

    public int? CarId { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    public bool? IsMainImage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("CarId")]
    [InverseProperty("CarImages")]
    public virtual Car? Car { get; set; }
}
