using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarSales.API.Models;

public partial class CarSalesDbContext : DbContext
{
    public CarSalesDbContext()
    {
    }

    public CarSalesDbContext(DbContextOptions<CarSalesDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<CarImage> CarImages { get; set; }

    public virtual DbSet<CarModel> CarModels { get; set; }

    public virtual DbSet<CarSpecification> CarSpecifications { get; set; }

    public virtual DbSet<ChatMessage> ChatMessages { get; set; }

    public virtual DbSet<ChatSession> ChatSessions { get; set; }

    public virtual DbSet<Specification> Specifications { get; set; }

    public virtual DbSet<TestDriveBooking> TestDriveBookings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\MSSQLSERVER16;Database=CarSalesDB;User Id=sa;Password=123456;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brands__DAD4F05EA7914108");

            entity.Property(e => e.BrandName).HasMaxLength(100);
            entity.Property(e => e.CountryOrigin).HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnType("ntext");
            entity.Property(e => e.LogoUrl).HasMaxLength(500);
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.CarId).HasName("PK__Cars__68A0342EB68BD647");

            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("ntext");
            entity.Property(e => e.FuelType).HasMaxLength(50);
            entity.Property(e => e.IsNew).HasDefaultValue(true);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.StockQuantity).HasDefaultValue(1);
            entity.Property(e => e.ThumbImage).HasMaxLength(500);
            entity.Property(e => e.Transmission).HasMaxLength(50);
            entity.Property(e => e.VersionName).HasMaxLength(100);

            entity.HasOne(d => d.Model).WithMany(p => p.Cars)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("FK_Cars_CarModels");
        });

        modelBuilder.Entity<CarImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__CarImage__7516F70CECA5733D");

            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsThumbnail).HasDefaultValue(false);

            entity.HasOne(d => d.Car).WithMany(p => p.CarImages)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK_CarImages_Cars");
        });

        modelBuilder.Entity<CarModel>(entity =>
        {
            entity.HasKey(e => e.ModelId).HasName("PK__CarModel__E8D7A12C4B36F51D");

            entity.Property(e => e.BodyType).HasMaxLength(50);
            entity.Property(e => e.ModelName).HasMaxLength(100);

            entity.HasOne(d => d.Brand).WithMany(p => p.CarModels)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("FK_CarModels_Brands");
        });

        modelBuilder.Entity<CarSpecification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CarSpeci__3214EC07067D198A");

            entity.Property(e => e.Value).HasMaxLength(255);

            entity.HasOne(d => d.Car).WithMany(p => p.CarSpecifications)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK_CarSpecs_Cars");

            entity.HasOne(d => d.Spec).WithMany(p => p.CarSpecifications)
                .HasForeignKey(d => d.SpecId)
                .HasConstraintName("FK_CarSpecs_Specs");
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__ChatMess__C87C0C9CA78797F4");

            entity.Property(e => e.Content).HasColumnType("ntext");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Session).WithMany(p => p.ChatMessages)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("FK_ChatMessages_Sessions");
        });

        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__ChatSess__C9F492905D7866DF");

            entity.Property(e => e.SessionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.ChatSessions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ChatSessions_Users");
        });

        modelBuilder.Entity<Specification>(entity =>
        {
            entity.HasKey(e => e.SpecId).HasName("PK__Specific__883D567B025BEB0A");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.SpecName).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(50);
        });

        modelBuilder.Entity<TestDriveBooking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__TestDriv__73951AEDCC93A64A");

            entity.Property(e => e.BookingDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.Note).HasColumnType("ntext");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Car).WithMany(p => p.TestDriveBookings)
                .HasForeignKey(d => d.CarId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Bookings_Cars");

            entity.HasOne(d => d.User).WithMany(p => p.TestDriveBookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Bookings_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C786DF216");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534F520C0C3").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("Customer");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
