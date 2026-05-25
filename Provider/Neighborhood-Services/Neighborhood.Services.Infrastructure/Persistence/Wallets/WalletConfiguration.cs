using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Wallets;
namespace Neighborhood.Services.Infrastructure.Persistence.Wallets
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Balance)
                .HasColumnType("decimal(18, 2)")
                .HasDefaultValue(0);
            builder.HasOne(w => w.User)
                .WithOne()
                .HasForeignKey<Wallet>(w => w.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasIndex(w => w.UserId).IsUnique();
        }
    }
}