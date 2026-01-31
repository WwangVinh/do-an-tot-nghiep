using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace oto.MyModels;

[Index("Username", Name = "UQ__Users__536C85E40C211271", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D10534F6AE4C51", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(255)]
    public string Username { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(255)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string? FullName { get; set; }

    [StringLength(15)]
    public string? Phone { get; set; }

    [StringLength(50)]
    public string Role { get; set; } = null!;

    [StringLength(500)]
    public string? Address { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Airecommendation> Airecommendations { get; set; } = new List<Airecommendation>();

    [InverseProperty("User")]
    public virtual ICollection<CarWishlist> CarWishlists { get; set; } = new List<CarWishlist>();

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("User")]
    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
}
