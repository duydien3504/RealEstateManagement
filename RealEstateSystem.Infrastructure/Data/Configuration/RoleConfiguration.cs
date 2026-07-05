using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.RoleId);

            builder.Property(r => r.NameRole)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.HasData(
                new Role { RoleId = Guid.Parse("a5e2f5b8-5f2b-426c-941f-897b6a18d1f8"), NameRole = RoleType.Admin },
                new Role { RoleId = Guid.Parse("b1f5d6c8-2e8b-4a5c-9c7d-3a1b8c2d4e6f"), NameRole = RoleType.Owner },
                new Role { RoleId = Guid.Parse("c2d6e7f8-3a9b-4c5d-8b7a-9a1b8c2d3e4f"), NameRole = RoleType.User }
            );
        }
    }
}
