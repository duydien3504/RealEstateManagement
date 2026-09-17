using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.CategoryId);

            builder.Property(c => c.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.HasData(
                new Category { CategoryId = Guid.Parse("d2b70f61-ef1a-4712-8e1f-7ff45b98f26a"), Name = "Căn hộ", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("100ad8c0-cf52-47cc-9e2c-29ef31ea81ff"), Name = "Nhà riêng", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("b56e6d1b-e523-455b-b9f2-897b2fa12e61"), Name = "Biệt thự", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("f56b27d4-8d99-4d6b-bd88-29ef5e1ba8c2"), Name = "Nhà phố thương mại", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("6be2f5c8-5f2b-426c-941f-897b6a18d2f1"), Name = "Đất nền", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("7be2f5c8-5f2b-426c-941f-897b6a18d2f2"), Name = "Đất nông nghiệp", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("8be2f5c8-5f2b-426c-941f-897b6a18d2f3"), Name = "Văn phòng", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("9be2f5c8-5f2b-426c-941f-897b6a18d2f4"), Name = "Mặt bằng kinh doanh", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("ace2f5c8-5f2b-426c-941f-897b6a18d2f5"), Name = "Kho xưởng", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("bce2f5c8-5f2b-426c-941f-897b6a18d2f6"), Name = "Phòng trọ", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("cce2f5c8-5f2b-426c-941f-897b6a18d2f7"), Name = "Căn hộ dịch vụ", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("dce2f5c8-5f2b-426c-941f-897b6a18d2f8"), Name = "Khách sạn", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("ece2f5c8-5f2b-426c-941f-897b6a18d2f9"), Name = "Resort", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("fce2f5c8-5f2b-426c-941f-897b6a18d2fa"), Name = "Homestay", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) },
                new Category { CategoryId = Guid.Parse("fae2f5c8-5f2b-426c-941f-897b6a18d2fb"), Name = "Nhà xưởng công nghiệp", CreatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc), UpdatedAt = new System.DateTime(2026, 1, 1, 0, 0, 0, System.DateTimeKind.Utc) }
            );
        }
    }
}
