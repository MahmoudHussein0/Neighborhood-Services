using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.HistoricalPrices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.HistoricalPrices
{
    public class HistoricalPriceConfiguration : IEntityTypeConfiguration<HistoricalPrice>
    {
        public void Configure(EntityTypeBuilder<HistoricalPrice> builder)
        {

            builder.Property(HP => HP.AveragePrice)
                    .IsRequired()
                    .HasColumnType("DECIMAL(18,2)");


            builder.Property(HP => HP.MaterialCost)
                    .IsRequired()
                    .HasColumnType("DECIMAL(18,2)");

            builder.Property(HP => HP.Region)
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(HP => HP.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");


            builder.HasOne(HP => HP.ProblemType)
                    .WithMany(PT => PT.HistoricalPricing)
                    .HasForeignKey(HP => HP.ProblemTypeId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(HP => HP.ProblemTypeId);
        }
    }
}
