using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CoreEntities.Models;

namespace SqlServer.DBContext
{

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

        public virtual DbSet<Article> Articles { get; set; }

        public virtual DbSet<Banner> Banners { get; set; }

        public virtual DbSet<Booking> Bookings { get; set; }

        public virtual DbSet<Car> Cars { get; set; }

        public virtual DbSet<CarImage> CarImages { get; set; }

        public virtual DbSet<CarSpecification> CarSpecifications { get; set; } = null!;

        public virtual DbSet<CarFeature> CarFeatures { get; set; } = null!;

        public virtual DbSet<CarWishlist> CarWishlists { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<ChatSession> ChatSessions { get; set; }

        public virtual DbSet<Feature> Features { get; set; }

        public virtual DbSet<LocationTaxis> LocationTaxes { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

        public virtual DbSet<Promotion> Promotions { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<Showroom> Showrooms { get; set; }

        public virtual DbSet<SystemAuditLog> SystemAuditLogs { get; set; }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserActivity> UserActivities { get; set; }

        public virtual DbSet<UserLogin> UserLogins { get; set; }

        public DbSet<ConsultationProfile> ConsultationProfiles { get; set; }
        public DbSet<CarInventory> CarInventories { get; set; }
        public DbSet<Consignment> Consignments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Airecommendation>(entity =>
            {
                entity.HasKey(e => e.RecommendationId).HasName("PK__AIRecomm__AA15BEE4630E9EA7");

                entity.ToTable("AIRecommendations");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Reason).HasMaxLength(500);

                entity.HasOne(d => d.Car).WithMany(p => p.Airecommendations)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__AIRecomme__CarId__5AEE82B9");

                entity.HasOne(d => d.User).WithMany(p => p.Airecommendations)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__AIRecomme__UserI__5BE2A6F2");
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.ArticleId).HasName("PK__Articles__9C6270E8685E14C2");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IsPublished).HasDefaultValue(true);
                entity.Property(e => e.Title).HasMaxLength(500);

                entity.HasOne(d => d.Author).WithMany(p => p.Articles)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Articles__Author__41EDCAC5");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.HasKey(e => e.BannerId).HasName("PK__Banners__32E86AD19CBE788F");

                entity.Property(e => e.BannerName)
                    .HasMaxLength(255)
                    .HasDefaultValue("Khuy?n mãi m?i");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.StartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Bookings__3214EC07B3AB1A46");

                entity.Property(e => e.BookingTime)
                    .HasMaxLength(10)
                    .IsUnicode(false);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.CustomerName).HasMaxLength(100);
                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");

                entity.HasOne(d => d.Car).WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bookings__CarId__2739D489");

                entity.HasOne(d => d.Showroom).WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.ShowroomId)
                    .HasConstraintName("FK__Bookings__Showro__3D2915A8");

                entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Bookings__UserId__282DF8C2");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(e => e.CarId).HasName("PK__Cars__68A0342EA480F904");

                entity.Property(e => e.Brand).HasMaxLength(100);
                entity.Property(e => e.Color).HasMaxLength(50);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeletedAt).HasColumnType("datetime");
                entity.Property(e => e.FuelType).HasMaxLength(50);
                entity.Property(e => e.Mileage).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Model).HasMaxLength(255);
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                //entity.HasMany(d => d.Features).WithMany(p => p.Cars)
                //    .UsingEntity<Dictionary<string, object>>(
                //        "CarFeature",
                //        r => r.HasOne<Feature>().WithMany()
                //            .HasForeignKey("FeatureId")
                //            .OnDelete(DeleteBehavior.ClientSetNull)
                //            .HasConstraintName("FK__CarFeatur__Featu__0C85DE4D"),
                //        l => l.HasOne<Car>().WithMany()
                //            .HasForeignKey("CarId")
                //            .OnDelete(DeleteBehavior.ClientSetNull)
                //            .HasConstraintName("FK__CarFeatur__CarId__0B91BA14"),
                //        j =>
                //        {
                //            j.HasKey("CarId", "FeatureId").HasName("PK__CarFeatu__E08204926F1DFBCC");
                //            j.ToTable("CarFeatures");
                //        });
            });

            modelBuilder.Entity<CarImage>(entity =>
            {
                entity.HasKey(e => e.CarImageId).HasName("PK__CarImage__614BE6AFD1BB03D9");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ImageUrl).HasMaxLength(255);

                entity.HasOne(d => d.Car).WithMany(p => p.CarImages)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__CarImages__CarId__5CD6CB2B");
            });

            modelBuilder.Entity<CarSpecification>(entity =>
            {
                entity.HasKey(e => e.SpecId).HasName("PK__CarSpeci__883D567BD7BFC457");

                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.SpecName).HasMaxLength(255);
                entity.Property(e => e.SpecValue).HasMaxLength(255);

                entity.HasOne(d => d.Car).WithMany(p => p.CarSpecifications)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CarSpecif__CarId__06CD04F7");
            });

            modelBuilder.Entity<CarFeature>(entity =>
            {
                // 1. DÒNG NÀY QUAN TRỌNG NHẤT ĐỂ TRỊ LỖI: Khai báo khóa chính kép
                entity.HasKey(e => new { e.CarId, e.FeatureId });

                // 2. Dây nối về bảng Car (Đoạn ní vừa viết)
                entity.HasOne(d => d.Car)
                      .WithMany(p => p.CarFeatures)
                      .HasForeignKey(d => d.CarId);

                // 3. Dây nối về bảng Feature (Khai báo nốt cho đủ bộ)
                entity.HasOne(d => d.Feature)
                      .WithMany(p => p.CarFeatures) // Lưu ý: Nếu trong model Feature  KHÔNG có ICollection<CarFeature> thì bỏ cái p => p.CarFeatures đi, chỉ để .WithMany() thôi nhé.
                      .HasForeignKey(d => d.FeatureId);
            });

            modelBuilder.Entity<CarWishlist>(entity =>
            {
                entity.HasKey(e => e.WishlistId).HasName("PK__CarWishl__233189EB685E97BB");

                entity.ToTable("CarWishlist");

                entity.Property(e => e.AddedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Car).WithMany(p => p.CarWishlists)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__CarWishli__CarId__5DCAEF64");

                entity.HasOne(d => d.User).WithMany(p => p.CarWishlists)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__CarWishli__UserI__5EBF139D");
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId).HasName("PK__ChatMess__C87C0C9C9F2D92FF");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.SenderType).HasMaxLength(20);

                entity.HasOne(d => d.Session).WithMany(p => p.ChatMessages)
                    .HasForeignKey(d => d.SessionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ChatMessa__Sessi__160F4887");
            });

            modelBuilder.Entity<ChatSession>(entity =>
            {
                entity.HasKey(e => e.SessionId).HasName("PK__ChatSess__C9F49290924C9141");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.GuestToken).HasMaxLength(255);
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("AI_Handling");

                entity.HasOne(d => d.AssignedToNavigation).WithMany(p => p.ChatSessionAssignedToNavigations)
                    .HasForeignKey(d => d.AssignedTo)
                    .HasConstraintName("FK__ChatSessi__Assig__123EB7A3");

                entity.HasOne(d => d.User).WithMany(p => p.ChatSessionUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__ChatSessi__UserI__114A936A");
            });

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.HasKey(e => e.FeatureId).HasName("PK__Features__82230BC9B29C0D63");

                entity.Property(e => e.FeatureName).HasMaxLength(255);
                entity.Property(e => e.Icon).HasMaxLength(255);
            });

            modelBuilder.Entity<LocationTaxis>(entity =>
            {
                entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA49744FC6D16");

                entity.Property(e => e.CityName).HasMaxLength(100);
                entity.Property(e => e.LicensePlateFee).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.RegistrationTaxPercent).HasColumnType("decimal(5, 2)");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFC3E9C951");

                entity.Property(e => e.OrderDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.Property(e => e.ShippingAddress).HasMaxLength(500);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Car).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__Orders__CarId__619B8048");

                entity.HasOne(d => d.Promotion).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PromotionId)
                    .HasConstraintName("FK__Orders__Promotio__29221CFB");

                entity.HasOne(d => d.User).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Orders__UserId__628FA481");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED06818F5EAB7E");

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Quantity).HasDefaultValue(1);

                entity.HasOne(d => d.Car).WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__OrderItem__CarId__5FB337D6");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderItem__Order__60A75C0F");
            });

            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId).HasName("PK__PaymentT__55433A6BD6EFEFD7");

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.TransactionDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Order).WithMany(p => p.PaymentTransactions)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__PaymentTr__Order__6383C8BA");
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.HasKey(e => e.PromotionId).HasName("PK__Promotio__52C42FCF8A6988C7");

                entity.HasIndex(e => e.Code, "UQ__Promotio__A25C5AA7A444C1CD").IsUnique();

                entity.Property(e => e.Code).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.DiscountPercentage).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE36435DC2");

                entity.Property(e => e.Comment).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Car).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__Reviews__CarId__6477ECF3");

                entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Reviews__UserId__656C112C");
            });

            modelBuilder.Entity<Showroom>(entity =>
            {
                entity.HasKey(e => e.ShowroomId).HasName("PK__Showroom__A7726CBBA913B30F");

                entity.Property(e => e.Hotline).HasMaxLength(20);
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Province)
                      .HasMaxLength(100)
                      .IsRequired(); // Bắt buộc phải có Tỉnh/Thành

                entity.Property(e => e.District)
                      .HasMaxLength(100);

                entity.Property(e => e.StreetAddress)
                      .HasMaxLength(300);
            });

            modelBuilder.Entity<SystemAuditLog>(entity =>
            {
                entity.HasKey(e => e.LogId).HasName("PK__SystemAu__5E548648E69FCEBF");

                entity.Property(e => e.ActionType).HasMaxLength(100);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IpAddress).HasMaxLength(50);
                entity.Property(e => e.RecordId).HasMaxLength(50);
                entity.Property(e => e.TableName).HasMaxLength(100);
                entity.Property(e => e.UserAgent).HasMaxLength(255);

                entity.HasOne(d => d.User).WithMany(p => p.SystemAuditLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SystemAud__UserI__1BC821DD");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C5DD76572");

                entity.HasIndex(e => e.Username, "UQ__Users__536C85E4014A9D7C").IsUnique();

                entity.HasIndex(e => e.Email, "UQ__Users__A9D105348B415492").IsUnique();

                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.DeletedAt).HasColumnType("datetime");
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.FullName).HasMaxLength(255);
                entity.Property(e => e.PasswordHash).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(15);
                entity.Property(e => e.Role).HasMaxLength(50);
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Active");
                entity.Property(e => e.Username).HasMaxLength(255);
            });

            modelBuilder.Entity<UserActivity>(entity =>
            {
                entity.HasKey(e => e.ActivityId).HasName("PK__UserActi__45F4A79164816C2C");

                entity.ToTable("UserActivity");

                entity.Property(e => e.ActivityDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.ActivityType).HasMaxLength(50);

                entity.HasOne(d => d.Car).WithMany(p => p.UserActivities)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__UserActiv__CarId__66603565");

                entity.HasOne(d => d.User).WithMany(p => p.UserActivities)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserActiv__UserI__6754599E");
            });

            modelBuilder.Entity<UserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK__UserLogi__2B2C5B522D37D021");

                entity.Property(e => e.LoginProvider).HasMaxLength(50);
                entity.Property(e => e.ProviderKey).HasMaxLength(255);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.User).WithMany(p => p.UserLogins)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserLogin__UserI__503BEA1C");
            });

            modelBuilder.Entity<CarInventory>()
                        .HasIndex(ci => new { ci.ShowroomId, ci.CarId })
                        .IsUnique();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}