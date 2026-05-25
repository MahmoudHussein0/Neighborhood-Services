using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.PromoCodes;
namespace Neighborhood.Services.Infrastructure.Persistence.PromoCodes
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.HasKey(pc => pc.Id);
            builder.Property(pc => pc.DiscountPercentage)
                .HasColumnType("decimal(5, 2)");
            builder.Property(pc => pc.Code)
                .HasMaxLength(50);
            builder.Property(pc => pc.MaxUses)
                .HasDefaultValue(1);
            builder.Property(pc => pc.UsedCount)
                .HasDefaultValue(0);
            builder.Property(pc => pc.ExpiresAt);
            builder.Property(pc => pc.IsActive)
                .HasDefaultValue(true);
            builder.Property(pc => pc.CreatedAt);
            builder.HasMany(pc => pc.Usages)
                .WithOne(u => u.PromoCode)
                .HasForeignKey(u => u.PromoCodeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}