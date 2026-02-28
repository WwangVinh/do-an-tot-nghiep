using System;
using System.Collections.Generic;

namespace CoreEntities.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? PasswordHash { get; set; }

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? Phone { get; set; }

    public string Role { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string Status { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    public string? AvatarUrl { get; set; }

    public virtual ICollection<Airecommendation> Airecommendations { get; set; } = new List<Airecommendation>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CarWishlist> CarWishlists { get; set; } = new List<CarWishlist>();

    public virtual ICollection<ChatSession> ChatSessionAssignedToNavigations { get; set; } = new List<ChatSession>();

    public virtual ICollection<ChatSession> ChatSessionUsers { get; set; } = new List<ChatSession>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<SystemAuditLog> SystemAuditLogs { get; set; } = new List<SystemAuditLog>();

    public virtual ICollection<UserActivity> UserActivities { get; set; } = new List<UserActivity>();

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();
}
