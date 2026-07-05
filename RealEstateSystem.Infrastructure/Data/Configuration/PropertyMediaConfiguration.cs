using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class PropertyMediaConfiguration : IEntityTypeConfiguration<PropertyMedia>
    {
        public void Configure(EntityTypeBuilder<PropertyMedia> builder)
        {
            builder.ToTable("PropertyMedia");

            builder.HasKey(pm => pm.MediaId);

            builder.Property(pm => pm.MediaUrl)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(pm => pm.MediaType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(pm => pm.Property)
                .WithMany(p => p.PropertyMedias)
                .HasForeignKey(pm => pm.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
