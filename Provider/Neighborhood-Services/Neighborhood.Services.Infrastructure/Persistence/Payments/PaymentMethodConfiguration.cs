using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Payments;
namespace Neighborhood.Services.Infrastructure.Persistence.Payments
{
    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.UserId);
            builder.Property(p => p.PaymentType)
                .HasConversion<string>();
            builder.Property(p => p.PaymentProvider)
                .HasConversion<string>();
            builder.Property(p => p.CreatedAt);
            builder.Property(p => p.ProviderToken);
            builder.Property(p => p.LastFourDigits)
                .HasMaxLength(4)
                .IsRequired(false);
            builder.Property(p => p.ExpiryMonth)
                .IsRequired(false);
            builder.Property(p => p.ExpiryYear)
                .IsRequired(false);
            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}