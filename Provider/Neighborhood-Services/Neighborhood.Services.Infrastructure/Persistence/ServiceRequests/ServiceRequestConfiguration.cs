using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.ServiceRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.ServiceRequests
{
    public class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
    {
        public void Configure(EntityTypeBuilder<ServiceRequest> builder)
        {
            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.Budget)
             .HasColumnType("decimal(18,2)");

            builder.Property(sr => sr.Status)
             .HasConversion<string>();

            builder.Property(sr => sr.Location)
            .HasColumnType("geography");

            builder.HasOne(sr => sr.Customer)
             .WithMany(c => c.ServiceRequests)
             .HasForeignKey(sr => sr.CustomerId)
             .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sr => sr.Category)
             .WithMany(c => c.ServiceRequests)
             .HasForeignKey(sr => sr.CategoryId)
             .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(sr => sr.ProblemType)
             .WithMany(p => p.ServiceRequests)
             .HasForeignKey(sr => sr.ProblemTypeId)
             .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
