using System;
using System.Collections.Generic;

namespace CarSales.API.Models;

public partial class Car
{
    public int CarId { get; set; }

    public int ModelId { get; set; }

    public string? VersionName { get; set; }

    public int? YearOfManufacture { get; set; }

    public decimal? Price { get; set; }

    public string? Color { get; set; }

    public string? FuelType { get; set; }

    public string? Transmission { get; set; }

    public int? StockQuantity { get; set; }

    public bool? IsNew { get; set; }

    public string? ThumbImage { get; set; }

    public string? Description { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<CarImage> CarImages { get; set; } = new List<CarImage>();

    public virtual ICollection<CarSpecification> CarSpecifications { get; set; } = new List<CarSpecification>();

    public virtual CarModel Model { get; set; } = null!;

    public virtual ICollection<TestDriveBooking> TestDriveBookings { get; set; } = new List<TestDriveBooking>();
}
