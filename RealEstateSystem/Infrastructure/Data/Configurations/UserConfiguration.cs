using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entities;

namespace RealEstateSystem.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.UserId);

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.UrlAvatar)
                .HasMaxLength(500);

            builder.HasOne<Role>()
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
