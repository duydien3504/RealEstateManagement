using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entities;
using RealEstateSystem.Domain.Enums;

namespace RealEstateSystem.Infrastructure.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.RoleId);

            builder.Property(r => r.RoleName)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.HasData(
                new Role(_roleId: 1, _roleName: RoleType.Admin),
                new Role(_roleId: 2, _roleName: RoleType.Owner),
                new Role(_roleId: 3, _roleName: RoleType.User)
            );
        }
    }
}
