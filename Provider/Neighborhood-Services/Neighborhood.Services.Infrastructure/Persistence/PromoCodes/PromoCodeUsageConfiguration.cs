using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.PromoCodes;
namespace Neighborhood.Services.Infrastructure.Persistence.PromoCodes
{
    public class PromoCodeUsageConfiguration : IEntityTypeConfiguration<PromoCodeUsage>
    {
        public void Configure(EntityTypeBuilder<PromoCodeUsage> builder)
        {
            builder.HasKey(pu => pu.Id);
            builder.Property(pu => pu.PromoCodeId);
            builder.Property(pu => pu.UserId);
            builder.Property(pu => pu.BookingId);
            builder.Property(pu => pu.UsedAt);
            builder.HasOne(pu => pu.PromoCode)
                .WithMany()
                .HasForeignKey(pu => pu.PromoCodeId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(pu => pu.Booking)
               .WithOne()
               .HasForeignKey<PromoCodeUsage>(pu => pu.BookingId)
               .OnDelete(DeleteBehavior.NoAction);
        }
    }
}