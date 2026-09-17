using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class OwnerUpgradePaymentConfiguration : IEntityTypeConfiguration<OwnerUpgradePayment>
    {
        public void Configure(EntityTypeBuilder<OwnerUpgradePayment> builder)
        {
            builder.ToTable("OwnerUpgradePayments", t => t.HasCheckConstraint(
                "CK_OwnerUpgradePayment_TransactionOrWalletTransaction",
                "(\"TransactionId\" IS NULL AND \"WalletTransactionId\" IS NOT NULL) OR (\"TransactionId\" IS NOT NULL AND \"WalletTransactionId\" IS NULL)"
            ));

            builder.HasKey(oup => oup.OwnerUpgradePaymentId);

            builder.Property(oup => oup.TransactionId)
                .IsRequired(false);

            builder.Property(oup => oup.WalletTransactionId)
                .IsRequired(false);

            builder.HasOne(oup => oup.Transaction)
                .WithOne(t => t.OwnerUpgradePayment)
                .HasForeignKey<OwnerUpgradePayment>(oup => oup.TransactionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oup => oup.OwnerProfileRequest)
                .WithMany(opr => opr.OwnerUpgradePayments)
                .HasForeignKey(oup => oup.OwnerProfileRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(oup => oup.WalletTransaction)
                .WithOne(wt => wt.OwnerUpgradePayment)
                .HasForeignKey<OwnerUpgradePayment>(oup => oup.WalletTransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
