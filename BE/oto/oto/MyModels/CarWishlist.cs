using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace oto.MyModels;

[Table("CarWishlist")]
public partial class CarWishlist
{
    [Key]
    public int WishlistId { get; set; }

    public int? UserId { get; set; }

    public int? CarId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? AddedAt { get; set; }

    [ForeignKey("CarId")]
    [InverseProperty("CarWishlists")]
    public virtual Car? Car { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("CarWishlists")]
    public virtual User? User { get; set; }
}
