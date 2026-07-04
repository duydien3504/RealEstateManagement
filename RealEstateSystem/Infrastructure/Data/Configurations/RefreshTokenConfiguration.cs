using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entities;

namespace RealEstateSystem.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshToken");
            builder.HasKey(x => x.RefreshTkId);

            builder.Property(x => x.Token)
                .IsRequired()
                .HasMaxLength(600);

            builder.Property(x => x.Expired)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Revoked);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
