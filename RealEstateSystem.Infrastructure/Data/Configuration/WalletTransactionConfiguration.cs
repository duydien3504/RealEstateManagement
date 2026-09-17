using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateSystem.Domain.Entity;

namespace RealEstateSystem.Infrastructure.Data.Configuration
{
    public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.ToTable("WalletTransactions");

            builder.HasKey(wt => wt.WalletTransactionId);

            builder.Property(wt => wt.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(wt => wt.TransactionType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(wt => wt.ReferenceType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(wt => wt.ReferenceId)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(wt => wt.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.HasOne(wt => wt.Wallet)
                .WithMany(w => w.WalletTransactions)
                .HasForeignKey(wt => wt.WalletId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
