using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Offers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Offers
{
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Price)
            .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Status)
            .HasConversion<string>();

            builder.HasOne(o => o.ServiceRequest)
             .WithMany(sr => sr.Offers)
             .HasForeignKey(o => o.ServiceRequestId)
             .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(o => o.Technician)
             .WithMany(t => t.Offers)
             .HasForeignKey(o => o.TechnicianId)
             .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
