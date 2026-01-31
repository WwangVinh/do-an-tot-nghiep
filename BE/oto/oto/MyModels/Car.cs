using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace oto.MyModels;

public partial class Car
{
    [Key]
    public int CarId { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [StringLength(255)]
    public string? Model { get; set; }

    public int? Year { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Price { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    public string? Description { get; set; }

    [StringLength(100)]
    public string? Brand { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? Mileage { get; set; }

    [StringLength(255)]
    public string? ImageUrl { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Car")]
    public virtual ICollection<Airecommendation> Airecommendations { get; set; } = new List<Airecommendation>();

    [InverseProperty("Car")]
    public virtual ICollection<CarImage> CarImages { get; set; } = new List<CarImage>();

    [InverseProperty("Car")]
    public virtual ICollection<CarWishlist> CarWishlists { get; set; } = new List<CarWishlist>();

    [InverseProperty("Car")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Car")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("Car")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Car")]
    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
}
