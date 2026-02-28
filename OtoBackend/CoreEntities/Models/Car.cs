using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;


namespace CoreEntities.Models;

public partial class Car
{
    public int CarId { get; set; }

    public string Name { get; set; } = null!;

    public string? Model { get; set; }

    public int? Year { get; set; }

    public decimal? Price { get; set; }

    public string? Color { get; set; }

    public string? Description { get; set; }

    public string? Brand { get; set; }

    public decimal? Mileage { get; set; }

    public string? ImageUrl { get; set; }

    //public string? Status { get; set; }

    public CarStatus? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? FuelType { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    [NotMapped] //thêm ảnh nhưng k tạo thêm cột trong database
    public IFormFile? ImageFile { get; set; }

    public virtual ICollection<Airecommendation> Airecommendations { get; set; } = new List<Airecommendation>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CarImage> CarImages { get; set; } = new List<CarImage>();

    public virtual ICollection<CarSpecification> CarSpecifications { get; set; } = new List<CarSpecification>();

    public virtual ICollection<CarWishlist> CarWishlists { get; set; } = new List<CarWishlist>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();

    public virtual ICollection<Feature> Features { get; set; } = new List<Feature>();
}
