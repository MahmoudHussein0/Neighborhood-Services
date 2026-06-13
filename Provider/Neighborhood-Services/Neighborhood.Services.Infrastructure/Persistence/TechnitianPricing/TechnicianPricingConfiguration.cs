using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.TechniciansPricing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.TechnitianPricing
{
    public class TechnicianPricingConfiguration : IEntityTypeConfiguration<TechnicianPricing>
    {
        public void Configure(EntityTypeBuilder<TechnicianPricing> builder)
        {
            builder.Property(TP => TP.MinPrice)
               .HasColumnType("DECIMAL(18,2)");


            builder.Property(TP => TP.MaxPrice)
                   .HasColumnType("DECIMAL(18,2)");


            builder.Property(TP => TP.IsDeleted)
                   .HasDefaultValue(false);


            //builder.HasQueryFilter(TP => !TP.IsDeleted);

            builder.Property(TP => TP.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");


            builder.HasOne(TP => TP.Technician)
                    .WithMany(TP => TP.TechnicianPricings)
                    .HasForeignKey(TP => TP.TechnicianId)
                    .OnDelete(DeleteBehavior.NoAction);



            builder.HasOne(TP => TP.ProblemType)
                   .WithMany(TP => TP.TechnicionPricing)
                   .HasForeignKey(TP => TP.ProblemTypeId)
                   .OnDelete(DeleteBehavior.NoAction);


            builder.HasIndex(TP => new { TP.TechnicianId , TP.ProblemTypeId})
                   .IsUnique();

        }
    }
}
