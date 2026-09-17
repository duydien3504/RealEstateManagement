using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.FullName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PasswordHash)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(u => u.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(StatusType.Inactive)
                .IsRequired();

            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(500);

            builder.Property(u => u.IsDeleted)
                .HasDefaultValue(false);

            builder.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed tài khoản Admin mặc định
            builder.HasData(
                new User
                {
                    UserId = Guid.Parse("d3b07384-d113-4a11-a8ff-60471cf4e3b7"),
                    RoleId = Guid.Parse("a5e2f5b8-5f2b-426c-941f-897b6a18d1f8"), // Admin RoleId
                    FullName = "Admin DuyDien",
                    Email = "duydien3504@gmail.com",
                    PasswordHash = "$2b$12$WMg1OuBBcbxSBPSTxUzRqu2NlV6f4EycDJ0ptuonDW3Wxf4rGK3Yu", // Hash của "Abcd1234@"
                    Status = StatusType.Active,
                    IsDeleted = false,
                    CreatedAt = new DateTime(2026, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2026, 7, 14, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
