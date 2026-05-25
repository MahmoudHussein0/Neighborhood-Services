using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Invoices;
namespace Neighborhood.Services.Infrastructure.Persistence.Invoices
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.BookingId);
            builder.Property(x => x.TransactionId);
            builder.Property(x => x.CustomerId);
            builder.Property(x => x.TechnicianId);
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Tax).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasDefaultValue(InvoiceStatus.Unpaid);
            builder.HasOne(x => x.Booking)
                .WithOne(b => b.Invoice)
                .HasForeignKey<Invoice>(x => x.BookingId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Transaction)
                .WithOne()
                .HasForeignKey<Invoice>(x => x.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
<<<<<<< Updated upstream
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Technician)
                .WithMany()
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.Cascade);
=======
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Technician)
                .WithMany()
                .HasForeignKey(x => x.TechnicianId)
                .OnDelete(DeleteBehavior.NoAction);
>>>>>>> Stashed changes
            builder.Property(x => x.PaidAt);
            builder.Property(x => x.VoidedAt);
            builder.Property(x => x.IssuedAt);
        }
    }
}