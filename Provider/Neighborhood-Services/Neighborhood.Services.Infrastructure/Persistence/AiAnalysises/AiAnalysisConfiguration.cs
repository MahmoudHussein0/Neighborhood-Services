using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.AiAnalyses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.AiAnalysises
{
    public class AiAnalysisConfiguration : IEntityTypeConfiguration<AiAnalysis>
    {
        public void Configure(EntityTypeBuilder<AiAnalysis> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.ConfidenceScore)
            .HasColumnType("decimal(5,2)");

            builder.Property(a => a.EstimatedMinPrice)
            .HasColumnType("decimal(18,2)");

            builder.Property(a => a.EstimatedMaxPrice)
            .HasColumnType("decimal(18,2)");

            builder.Property(a => a.SeverityLevel)
            .HasConversion<string>();

            builder.HasOne(a => a.Booking)
            .WithOne(b => b.AiAnalysis)
            .HasForeignKey<AiAnalysis>(a => a.BookingId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);


        }
    }
}
