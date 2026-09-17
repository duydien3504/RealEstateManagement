using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.ToTable("Amenities");

            builder.HasKey(a => a.AmenityId);

            builder.Property(a => a.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(a => a.IconUrl)
                .HasMaxLength(500);

            builder.HasData(
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2f1"), Name = "Hồ bơi", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2f2"), Name = "Phòng gym", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2f3"), Name = "Công viên", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2f4"), Name = "Khu vui chơi trẻ em", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2f5"), Name = "Siêu thị", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2f6"), Name = "Trung tâm thương mại", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2f7"), Name = "Trường học", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2f8"), Name = "Bệnh viện", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2f9"), Name = "Nhà thuốc", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2fa"), Name = "Bãi đậu xe", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2fb"), Name = "Thang máy", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2fc"), Name = "Máy phát điện", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2fd"), Name = "Bảo vệ 24/7", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2fe"), Name = "Camera an ninh", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d2ff"), Name = "Lễ tân", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d301"), Name = "BBQ", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d302"), Name = "Sân tennis", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d303"), Name = "Sân bóng rổ", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d304"), Name = "Sân cầu lông", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d305"), Name = "Spa", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d306"), Name = "Sauna", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d307"), Name = "Hồ cảnh quan", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d308"), Name = "Đường chạy bộ", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d309"), Name = "Wifi miễn phí", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Amenity { AmenityId = Guid.Parse("4fe2f5c8-5f2b-426c-941f-897b6a18d30a"), Name = "Điều hòa", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) }
            );
        }
    }
}
