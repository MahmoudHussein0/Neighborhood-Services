using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Reviews;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Reviews.Configurations
{
    public class ReviewAnalysisConfiguration : IEntityTypeConfiguration<ReviewAnalysis>
    {
        public void Configure(EntityTypeBuilder<ReviewAnalysis> builder)
        {
            builder.ToTable("ReviewAnalyses");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .UseIdentityColumn();

            builder.Property(a => a.ReviewId)
                .IsRequired();

            builder.Property(a => a.Sentiment)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.IsFlagged)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(a => a.QualityScore)
                .IsRequired()
                .HasPrecision(4, 2); // e.g. 99.99

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.HasIndex(a => a.ReviewId)
                .IsUnique()
                .HasDatabaseName("IX_ReviewAnalyses_ReviewId");

            builder.HasIndex(a => a.IsFlagged)
                .HasDatabaseName("IX_ReviewAnalyses_IsFlagged");

            builder.HasIndex(a => a.Sentiment)
                .HasDatabaseName("IX_ReviewAnalyses_Sentiment");
        }
    }
}
