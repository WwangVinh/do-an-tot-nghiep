using System;
using System.Collections.Generic;

namespace CarSales.API.Models;

public partial class TestDriveBooking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int CarId { get; set; }

    public string? CustomerName { get; set; }

    public string? CustomerPhone { get; set; }

    public DateTime BookingDate { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Car Car { get; set; } = null!;

    public virtual User? User { get; set; }
}
