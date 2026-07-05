using System.Reflection;
using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<OtpVerification> OtpVerifications { get; set; } = null!;
        public DbSet<OwnerProfileRequest> OwnerProfileRequests { get; set; } = null!;
        public DbSet<Province> Provinces { get; set; } = null!;
        public DbSet<Ward> Wards { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Amenity> Amenities { get; set; } = null!;
        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<PropertyAmenity> PropertyAmenities { get; set; } = null!;
        public DbSet<PropertyMedia> PropertyMedias { get; set; } = null!;
        public DbSet<UserFavorite> UserFavorites { get; set; } = null!;
        public DbSet<PropertyRating> PropertyRatings { get; set; } = null!;
        public DbSet<PropertyReport> PropertyReports { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
