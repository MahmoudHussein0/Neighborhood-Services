using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.CancellationPolicies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.CancellationPolicies
{
    public class CancellationPolicyConfiguration : IEntityTypeConfiguration<CancellationPolicy>
    {
        public void Configure(EntityTypeBuilder<CancellationPolicy> builder)
        {
            builder.HasKey(cp => cp.Id);
            builder.Property(cp => cp.PenaltyPct)
            .HasColumnType("decimal(5,2)");

            //builder.Property(cp => cp.PenaltyPct)
            //    .HasColumnType("decimal(5,2)");
            builder.Property(cp => cp.AppliesTo).HasConversion<string>();
        }
    }
}
