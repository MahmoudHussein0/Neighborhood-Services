using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.RecurringBookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.RecurringBookings
{
    public class RecurringBookingConfiguration : IEntityTypeConfiguration<RecurringBooking>
    {
        public void Configure(EntityTypeBuilder<RecurringBooking> builder)
        {
            builder.HasKey(rb => rb.Id);

            builder.Property(rb => rb.Pattern)
            .HasConversion<string>();

            builder.HasOne(rb => rb.Customer)
            .WithMany(c => c.RecurringBookings)
            .HasForeignKey(rb => rb.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(rb => rb.Technician)
             .WithMany(t => t.RecurringBookings)
             .HasForeignKey(rb => rb.TechnicianId)
             .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(rb => rb.ProblemType)
             .WithMany()
             .HasForeignKey(rb => rb.ProblemTypeId)
             .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
