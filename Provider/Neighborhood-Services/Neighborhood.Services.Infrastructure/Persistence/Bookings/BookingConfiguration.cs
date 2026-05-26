using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Bookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Bookings
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);


            builder.HasQueryFilter(b => !b.IsDeleted);

            builder.Property(b => b.EstimatedPrice)
            .HasColumnType("decimal(18,2)");

            builder.Property(b => b.FinalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.BookingType)
             .HasConversion<string>()
            .HasMaxLength(50);

            builder.Property(b => b.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(b => b.Location)
            .HasColumnType("geography");

            builder.HasOne(b => b.Customer)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.Technician)
            .WithMany(t => t.Bookings)
            .HasForeignKey(b => b.TechnicianId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.ProblemType)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.ProblemTypeId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.Offer)
            .WithOne(o => o.Booking)
            .HasForeignKey<Booking>(b => b.OfferId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.ServiceRequest)
            .WithOne(s => s.Booking)
            .HasForeignKey<Booking>(b => b.ServiceRequestId)
              .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.PromoCode)
            .WithMany(p => p.Bookings)
            .HasForeignKey(b => b.PromoCodeId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.RecurringBooking)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RecurringBookingId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(b => b.CancelledByUser)
            .WithMany()
            .HasForeignKey(b => b.CancelledBy)
            .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
