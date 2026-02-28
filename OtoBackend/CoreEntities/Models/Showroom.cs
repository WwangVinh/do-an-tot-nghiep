using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class Showroom
{
    public int ShowroomId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string? Hotline { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
