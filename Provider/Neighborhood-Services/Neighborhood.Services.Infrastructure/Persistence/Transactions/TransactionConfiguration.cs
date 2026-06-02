using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Infrastructure.Persistence.Transactions
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)");
            builder.HasIndex(t => t.FromWalletId);
            builder.HasIndex(t => t.ToWalletId);
            builder.HasIndex(t => t.PaymentMethodId);
            builder.HasIndex(t => t.OriginalTransactionId);
            builder.Property(t => t.Fee)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);
            builder.Property(t => t.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("EGP");
            builder.Property(t => t.Type)
                .HasConversion<string>();
            builder.Property(t => t.Status)
                .HasConversion<string>()
                .HasDefaultValue(TransactionStatus.Pending);
            builder.HasOne(t => t.FromWallet)
                .WithMany(w => w.OutgoingTransactions)
                .HasForeignKey(t => t.FromWalletId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(t => t.PaymentMethod)
                .WithMany()
                .HasForeignKey(t => t.PaymentMethodId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(t => t.ToWallet)
            .WithMany(w => w.IncomingTransactions)
            .HasForeignKey(t => t.ToWalletId)
            .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
