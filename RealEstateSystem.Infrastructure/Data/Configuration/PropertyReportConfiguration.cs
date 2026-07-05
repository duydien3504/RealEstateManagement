using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class PropertyReportConfiguration : IEntityTypeConfiguration<PropertyReport>
    {
        public void Configure(EntityTypeBuilder<PropertyReport> builder)
        {
            builder.ToTable("PropertyReports");

            builder.HasKey(pr => pr.ReportId);

            builder.Property(pr => pr.Reason)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(pr => pr.Details)
                .HasMaxLength(2000);

            builder.Property(pr => pr.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(pr => pr.User)
                .WithMany(u => u.PropertyReports)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pr => pr.Property)
                .WithMany(p => p.PropertyReports)
                .HasForeignKey(pr => pr.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
