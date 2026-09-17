using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class PropertyPaymentConfiguration : IEntityTypeConfiguration<PropertyPayment>
    {
        public void Configure(EntityTypeBuilder<PropertyPayment> builder)
        {
            builder.ToTable("PropertyPayments", t => t.HasCheckConstraint(
                "CK_PropertyPayment_TransactionOrWalletTransaction",
                "(\"TransactionId\" IS NULL AND \"WalletTransactionId\" IS NOT NULL) OR (\"TransactionId\" IS NOT NULL AND \"WalletTransactionId\" IS NULL)"
            ));

            builder.HasKey(pp => pp.PropertyPaymentId);

            builder.Property(pp => pp.TransactionId)
                .IsRequired(false);

            builder.Property(pp => pp.WalletTransactionId)
                .IsRequired(false);

            builder.HasOne(pp => pp.Transaction)
                .WithOne(t => t.PropertyPayment)
                .HasForeignKey<PropertyPayment>(pp => pp.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pp => pp.Property)
                .WithMany(p => p.PropertyPayments)
                .HasForeignKey(pp => pp.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pp => pp.WalletTransaction)
                .WithOne(wt => wt.PropertyPayment)
                .HasForeignKey<PropertyPayment>(pp => pp.WalletTransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
