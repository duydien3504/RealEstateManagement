using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class PropertyRatingConfiguration : IEntityTypeConfiguration<PropertyRating>
    {
        public void Configure(EntityTypeBuilder<PropertyRating> builder)
        {
            builder.ToTable("PropertyRatings");

            builder.HasKey(pr => pr.RatingId);

            builder.Property(pr => pr.RatingValue)
                .IsRequired();

            builder.Property(pr => pr.Comment)
                .HasMaxLength(1000);

            builder.HasOne(pr => pr.User)
                .WithMany(u => u.PropertyRatings)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pr => pr.Property)
                .WithMany(p => p.PropertyRatings)
                .HasForeignKey(pr => pr.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
