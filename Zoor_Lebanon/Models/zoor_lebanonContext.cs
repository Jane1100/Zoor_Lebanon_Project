using Microsoft.EntityFrameworkCore;

namespace Zoor_Lebanon.Models
{
    public partial class zoor_lebanonContext : DbContext
    {
        public zoor_lebanonContext()
        {
        }

        public zoor_lebanonContext(DbContextOptions<zoor_lebanonContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ActivitySchedule> ActivitySchedules { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Country> Countries { get; set; } = null!;
        public virtual DbSet<Coupon> Coupons { get; set; } = null!;
        public virtual DbSet<Location> Locations { get; set; } = null!;
        public virtual DbSet<Package> Packages { get; set; } = null!;
        public virtual DbSet<PackageType> PackageTypes { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Preference> Preferences { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<Reward> Rewards { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<State> States { get; set; } = null!;
        public virtual DbSet<TourOperator> TourOperators { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserCoupon> UserCoupons { get; set; } = null!;
        public virtual DbSet<UserPreference> UserPreferences { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;database=zoor_lebanon;user=root;password=password", ServerVersion.Parse("9.0.0-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<ActivitySchedule>(entity =>
            {
                entity.HasKey(e => e.ActivityId)
                    .HasName("PRIMARY");

                entity.ToTable("activity_schedule");

                entity.HasIndex(e => e.PackageId, "package_id");

                entity.Property(e => e.ActivityId).HasColumnName("activity_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.FromTime)
                    .HasColumnType("timestamp")
                    .HasColumnName("from_time");

                entity.Property(e => e.PackageId).HasColumnName("package_id");

                entity.Property(e => e.ToTime)
                    .HasColumnType("timestamp")
                    .HasColumnName("to_time");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.ActivitySchedules)
                    .HasForeignKey(d => d.PackageId)
                    .HasConstraintName("activity_schedule_ibfk_1");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("booking");

                entity.HasIndex(e => e.PackageId, "package_id");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.BookingId).HasColumnName("booking_id");

                entity.Property(e => e.BookingDate)
                    .HasColumnType("timestamp")
                    .HasColumnName("booking_date")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CancellationStatus)
                      .HasConversion<bool>() // Converts TINYINT to bool
                      .HasColumnName("cancellation_status");

                entity.Property(e => e.PackageId).HasColumnName("package_id");

                entity.Property(e => e.PaymentStatus)
                    .HasColumnType("enum('pending','paid')")
                    .HasColumnName("payment_status");

                entity.Property(e => e.TotalPrice)
                    .HasPrecision(10, 2)
                    .HasColumnName("total_price");

                entity.Property(e => e.TravelDate).HasColumnName("travel_date");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.PackageId)
                    .HasConstraintName("booking_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("booking_ibfk_1");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.ToTable("city");

                entity.HasIndex(e => e.StateId, "state_id");

                entity.Property(e => e.CityId).HasColumnName("city_id");

                entity.Property(e => e.City1)
                    .HasMaxLength(255)
                    .HasColumnName("city");

                entity.Property(e => e.StateId).HasColumnName("state_id");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Cities)
                    .HasForeignKey(d => d.StateId)
                    .HasConstraintName("city_ibfk_1");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("country");

                entity.Property(e => e.CountryId).HasColumnName("country_id");

                entity.Property(e => e.Country1)
                    .HasMaxLength(255)
                    .HasColumnName("country");
            });

            modelBuilder.Entity<Coupon>(entity =>
            {
                entity.ToTable("coupon");

                entity.Property(e => e.CouponId).HasColumnName("coupon_id");

                entity.Property(e => e.CouponCode)
                    .HasMaxLength(255)
                    .HasColumnName("coupon_code");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.DiscountValue)
                    .HasPrecision(10, 2)
                    .HasColumnName("discount_value");

                entity.Property(e => e.ExpirationDate).HasColumnName("expiration_date");

                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.Property(e => e.PointsCost).HasColumnName("points_cost");
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("location");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .HasColumnName("city");

                entity.Property(e => e.State)
                    .HasMaxLength(255)
                    .HasColumnName("state");
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.ToTable("package");

                entity.HasIndex(e => e.LocationId, "location_id");

                entity.HasIndex(e => e.PackageTypeId, "package_type_id");

                entity.Property(e => e.PackageId).HasColumnName("package_id");

                entity.Property(e => e.AvailableSpots).HasColumnName("available_spots");

                entity.Property(e => e.AverageDuration)
                    .HasPrecision(5, 2)
                    .HasColumnName("average_duration");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.Property(e => e.LocationId).HasColumnName("location_id");

                entity.Property(e => e.PackageName)
                    .HasMaxLength(255)
                    .HasColumnName("package_name");

                entity.Property(e => e.PackageTypeId).HasColumnName("package_type_id");

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.Property(e => e.TotalSpots).HasColumnName("total_spots");

                entity.Property(e => e.UnitPrice)
                    .HasPrecision(10, 2)
                    .HasColumnName("unit_price");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.Packages)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("package_ibfk_1");

                entity.HasOne(d => d.PackageType)
                    .WithMany(p => p.Packages)
                    .HasForeignKey(d => d.PackageTypeId)
                    .HasConstraintName("package_ibfk_2");
            });

            modelBuilder.Entity<PackageType>(entity =>
            {
                entity.ToTable("package_type");

                entity.Property(e => e.PackageTypeId).HasColumnName("package_type_id");

                entity.Property(e => e.PackageType1)
                    .HasMaxLength(255)
                    .HasColumnName("package_type");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.ToTable("payment");

                entity.HasIndex(e => e.BookingId, "booking_id");

                entity.Property(e => e.PaymentId).HasColumnName("payment_id");

                entity.Property(e => e.Amount)
                    .HasPrecision(10, 2)
                    .HasColumnName("amount");

                entity.Property(e => e.BookingId).HasColumnName("booking_id");

                entity.Property(e => e.PaymentDate).HasColumnName("payment_date");

                entity.Property(e => e.PaymentMethod)
                    .HasColumnType("enum('credit_card','paypal','bank_transfer')")
                    .HasColumnName("payment_method");

                entity.Property(e => e.Status)
                    .HasColumnType("enum('successful','failed')")
                    .HasColumnName("status");

                entity.HasOne(d => d.Booking)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.BookingId)
                    .HasConstraintName("payment_ibfk_1");
            });

            modelBuilder.Entity<Preference>(entity =>
            {
                entity.ToTable("preference");

                entity.Property(e => e.PreferenceId).HasColumnName("preference_id");

                entity.Property(e => e.PreferenceDescription)
                    .HasMaxLength(255)
                    .HasColumnName("preference_description");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("review");

                entity.HasIndex(e => e.PackageId, "package_id");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.ReviewId).HasColumnName("review_id");

                entity.Property(e => e.PackageId).HasColumnName("package_id");

                entity.Property(e => e.PublishedStatus)
                    .HasColumnType("enum('published','unpublished')")
                    .HasColumnName("published_status");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.ReviewDescription)
                    .HasColumnType("text")
                    .HasColumnName("review_description");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Package)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.PackageId)
                    .HasConstraintName("review_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("review_ibfk_1");
            });

            modelBuilder.Entity<Reward>(entity =>
            {
                entity.HasKey(e => e.RewardsId)
                    .HasName("PRIMARY");

                entity.ToTable("rewards");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.RewardsId).HasColumnName("rewards_id");

                entity.Property(e => e.CurrentBalance).HasColumnName("current_balance");

                entity.Property(e => e.PointsEarned).HasColumnName("points_earned");

                entity.Property(e => e.PointsSpent).HasColumnName("points_spent");

                entity.Property(e => e.TransactionDate).HasColumnName("transaction_date");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Rewards)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("rewards_ibfk_1");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(255)
                    .HasColumnName("role_name");
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("state");

                entity.HasIndex(e => e.CountryId, "country_id");

                entity.Property(e => e.StateId).HasColumnName("state_id");

                entity.Property(e => e.CountryId).HasColumnName("country_id");

                entity.Property(e => e.State1)
                    .HasMaxLength(255)
                    .HasColumnName("state");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("state_ibfk_1");
            });

            modelBuilder.Entity<TourOperator>(entity =>
            {
                entity.HasKey(e => e.OperatorId)
                    .HasName("PRIMARY");

                entity.ToTable("tour_operator");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.OperatorId).HasColumnName("operator_id");

                entity.Property(e => e.BusinessPhone)
                    .HasMaxLength(15)
                    .HasColumnName("business_phone")
                    .HasComment("Lebanon phone number starting with +961");

                entity.Property(e => e.CompanyName)
                    .HasMaxLength(255)
                    .HasColumnName("company_name");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.TourOperators)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("tour_operator_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.CityId, "city_id");

                entity.HasIndex(e => e.RoleId, "role_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.CityId).HasColumnName("city_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp")
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Dob).HasColumnName("dob");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email")
                    .HasComment("User email with global validation");

                entity.Property(e => e.Firstname)
                    .HasMaxLength(255)
                    .HasColumnName("firstname");

                entity.Property(e => e.Lastname)
                    .HasMaxLength(255)
                    .HasColumnName("lastname");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .HasColumnName("password_hash");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .HasColumnName("phone_number")
                    .HasComment("Global phone number validation");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.CityId)
                    .HasConstraintName("user_ibfk_1");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("user_ibfk_2");
            });

            modelBuilder.Entity<UserCoupon>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CouponId })
                    .HasName("PRIMARY")
                    .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                entity.ToTable("user_coupon");

                entity.HasIndex(e => e.CouponId, "coupon_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.CouponId).HasColumnName("coupon_id");

                entity.Property(e => e.IsRedeemed).HasColumnName("is_redeemed");

                entity.Property(e => e.UnlockDate).HasColumnName("unlock_date");

                entity.HasOne(d => d.Coupon)
                    .WithMany(p => p.UserCoupons)
                    .HasForeignKey(d => d.CouponId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_coupon_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserCoupons)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("user_coupon_ibfk_1");
            });

            modelBuilder.Entity<UserPreference>(entity =>
            {
                entity.ToTable("user_preference");

                entity.HasIndex(e => e.PreferenceId, "preference_id");

                entity.HasIndex(e => e.UserId, "user_id");

                entity.Property(e => e.UserPreferenceId).HasColumnName("user_preference_id");

                entity.Property(e => e.PreferenceId).HasColumnName("preference_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Preference)
                    .WithMany(p => p.UserPreferences)
                    .HasForeignKey(d => d.PreferenceId)
                    .HasConstraintName("user_preference_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserPreferences)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("user_preference_ibfk_1");
            });



            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
