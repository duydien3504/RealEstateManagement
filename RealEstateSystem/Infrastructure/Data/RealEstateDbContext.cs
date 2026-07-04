using Microsoft.EntityFrameworkCore;
using RealEstateSystem.Domain.Entities;
using RealEstateSystem.Infrastructure.Data.Configurations;
using System.Reflection;

namespace RealEstateSystem.Infrastructure.Data
{
    public class RealEstateDbContext: DbContext
    {
        public RealEstateDbContext(DbContextOptions<RealEstateDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<OtpVerification> OtpVerifications {  get; set; }
        public DbSet<OwnerProfileRequest> OwnerProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder _ModelBuilder)
        {
            base.OnModelCreating(_ModelBuilder);
            _ModelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
