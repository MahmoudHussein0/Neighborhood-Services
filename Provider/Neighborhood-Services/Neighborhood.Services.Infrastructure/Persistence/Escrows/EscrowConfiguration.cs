using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Escrows;
namespace Neighborhood.Services.Infrastructure.Persistence.Escrows
{
    public class EscrowConfiguration : IEntityTypeConfiguration<Escrow>
    {
        public void Configure(EntityTypeBuilder<Escrow> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)");
            builder.Property(e => e.Status)
                .HasConversion<string>()
                .HasDefaultValue(EscrowStatus.Held);
            builder.Property(e => e.HeldAt);
            builder.Property(e => e.ReleasedAt).IsRequired(false);
            builder.HasOne(e => e.Wallet)
                .WithMany()
                .HasForeignKey(e => e.WalletId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e => e.Booking)
                .WithOne(b=>b.Escrow)
                .HasForeignKey<Escrow>(e => e.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}