using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.CustomerAddresses;

namespace Neighborhood.Services.Infrastructure.Persistence.CustomerAddresses
{
    public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerAddress> builder)
        {
            builder.HasKey(ca => ca.Id);

            builder.Property(ca => ca.Label)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(ca => ca.Address)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(ca => ca.Location)
                .HasColumnType("geography")
                .IsRequired();

            builder.Property(ca => ca.IsDefault)
                .HasDefaultValue(false);

            builder.Property(ca => ca.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(ca => ca.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(ca => ca.ApplicationUserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.HasOne(ca => ca.Customer)
                .WithMany(c => c.CustomerAddresses)
                .HasForeignKey(ca => ca.CustomerId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
