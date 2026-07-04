using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entities;

namespace RealEstateSystem.Infrastructure.Data.Configurations
{
    public class OtpVerificationConfiguration : IEntityTypeConfiguration<OtpVerification>
    {
        public void Configure(EntityTypeBuilder<OtpVerification> builder)
        {
            builder.ToTable("OtpVerifications");

            builder.HasKey(x => x.OtpId);

            builder.Property(x => x.OtpId)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.Purpose)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.HasOne<User>()
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
