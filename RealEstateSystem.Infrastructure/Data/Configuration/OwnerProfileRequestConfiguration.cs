using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class OwnerProfileRequestConfiguration : IEntityTypeConfiguration<OwnerProfileRequest>
    {
        public void Configure(EntityTypeBuilder<OwnerProfileRequest> builder)
        {
            builder.ToTable("OwnerProfileRequests");

            builder.HasKey(o => o.RequestId);

            builder.Property(o => o.IdCardNumber)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(o => o.RawDocumentsUrl)
                .HasMaxLength(500);

            builder.Property(o => o.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(o => o.RejectReason)
                .HasMaxLength(500);

            builder.HasOne(o => o.User)
                .WithMany(u => u.OwnerProfileRequests)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.ApprovedByAdmin)
                .WithMany(u => u.ApprovedOwnerProfileRequests)
                .HasForeignKey(o => o.ApprovedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
