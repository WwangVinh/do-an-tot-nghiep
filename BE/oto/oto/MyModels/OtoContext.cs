using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace oto.MyModels;

public partial class OtoContext : DbContext
{
    public OtoContext()
    {
    }

    public OtoContext(DbContextOptions<OtoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airecommendation> Airecommendations { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarImage> CarImages { get; set; }

    public virtual DbSet<CarWishlist> CarWishlists { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airecommendation>(entity =>
        {
            entity.HasKey(e => e.RecommendationId).HasName("PK__AIRecomm__AA15BEE4FA7604A4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Car).WithMany(p => p.Airecommendations).HasConstraintName("FK__AIRecomme__CarId__656C112C");

            entity.HasOne(d => d.User).WithMany(p => p.Airecommendations).HasConstraintName("FK__AIRecomme__UserI__6477ECF3");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Cars__68A0342E27CAA7CA");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<CarImage>(entity =>
        {
            entity.HasKey(e => e.CarImageId).HasName("PK__CarImage__614BE6AF6F9F8023");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Car).WithMany(p => p.CarImages).HasConstraintName("FK__CarImages__CarId__693CA210");
        });

        modelBuilder.Entity<CarWishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__CarWishl__233189EB687A0951");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Car).WithMany(p => p.CarWishlists).HasConstraintName("FK__CarWishli__CarId__76969D2E");

            entity.HasOne(d => d.User).WithMany(p => p.CarWishlists).HasConstraintName("FK__CarWishli__UserI__75A278F5");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFD3112CA0");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Car).WithMany(p => p.Orders).HasConstraintName("FK__Orders__CarId__5441852A");

            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("FK__Orders__UserId__534D60F1");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED068114ADE69A");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Car).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__CarId__59063A47");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK__OrderItem__Order__5812160E");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__PaymentT__55433A6BE508D9B6");

            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Order).WithMany(p => p.PaymentTransactions).HasConstraintName("FK__PaymentTr__Order__71D1E811");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__52C42FCF5391CF90");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE42AC94F0");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Car).WithMany(p => p.Reviews).HasConstraintName("FK__Reviews__CarId__6D0D32F4");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews).HasConstraintName("FK__Reviews__UserId__6E01572D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CFCE2DDF4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK__UserActi__45F4A791C286EB7F");

            entity.Property(e => e.ActivityDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Car).WithMany(p => p.UserActivities).HasConstraintName("FK__UserActiv__CarId__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.UserActivities).HasConstraintName("FK__UserActiv__UserI__5FB337D6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
