using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entities;

namespace RealEstateSystem.Infrastructure.Data.Configurations
{
    public class OwnerProfileRequestConfiguration : IEntityTypeConfiguration<OwnerProfileRequest>
    {
        public void Configure(EntityTypeBuilder<OwnerProfileRequest> builder)
        {
            builder.ToTable("OwnerProfileRequests");

            builder.HasKey(x => x.RequestId);
            builder.Property(x => x.RequestId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ApprovedByAdmin)
                .WithMany()
                .HasForeignKey(x => x.ApprovedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
