using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.AgentLogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.AgentLogs
{
    public class AgentLogConfiguration : IEntityTypeConfiguration<AgentLog>
    {
        public void Configure(EntityTypeBuilder<AgentLog> builder)
        {

            builder.HasKey(a => a.Id);

            builder.Property(a => a.AgentType)
           .HasConversion<string>();

            builder.Property(a => a.ReferenceType)
             .HasConversion<string>();

            builder.Property(a => a.Action)
              .HasMaxLength(200);

            builder.Property(a => a.Input)
             .HasColumnType("nvarchar(max)");

            builder.Property(a => a.Output)
            .HasColumnType("nvarchar(max)");
        }
    }
}
