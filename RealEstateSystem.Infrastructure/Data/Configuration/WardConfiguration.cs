using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class WardConfiguration : IEntityTypeConfiguration<Ward>
    {
        public void Configure(EntityTypeBuilder<Ward> builder)
        {
            builder.ToTable("Wards");

            builder.HasKey(w => w.WardId);

            builder.Property(w => w.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.HasOne(w => w.Province)
                .WithMany(p => p.Wards)
                .HasForeignKey(w => w.ProvinceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
