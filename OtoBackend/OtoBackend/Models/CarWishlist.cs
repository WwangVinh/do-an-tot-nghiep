using System;
using System.Collections.Generic;

namespace OtoBackend.Models;

public partial class CarWishlist
{
    public int WishlistId { get; set; }

    public int? UserId { get; set; }

    public int? CarId { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Car? Car { get; set; }

    public virtual User? User { get; set; }
}
