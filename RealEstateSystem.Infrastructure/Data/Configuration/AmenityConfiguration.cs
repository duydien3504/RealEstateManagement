using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.ToTable("Amenities");

            builder.HasKey(a => a.AmenityId);

            builder.Property(a => a.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(a => a.IconUrl)
                .HasMaxLength(500);
        }
    }
}
