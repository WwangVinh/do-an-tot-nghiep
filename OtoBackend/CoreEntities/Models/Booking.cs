using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int CarId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateOnly BookingDate { get; set; }

    public string? BookingTime { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UserId { get; set; }

    public string Status { get; set; } = null!;

    public int? ShowroomId { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual Showroom? Showroom { get; set; }

    public virtual User? User { get; set; }
}
