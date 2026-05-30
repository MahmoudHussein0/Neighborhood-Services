using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.AvilabilitiesException;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.AvilabilitiesException
{
    public class AvailabilityExceptionConfiguration : IEntityTypeConfiguration<AvailabilityException>
    {
        public void Configure(EntityTypeBuilder<AvailabilityException> builder)
        {
            builder.Property(AE => AE.IsAvailable)
                    .HasDefaultValue(false);

            builder.Property(AE => AE.Reason)
                    .HasMaxLength(250);

            builder.Property(AE => AE.IsDeleted)
                    .HasDefaultValue(false);

            builder.HasQueryFilter(AE => !AE.IsDeleted);


            builder.Property(AE => AE.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");


            builder.HasOne(AE => AE.Technician)
                    .WithMany()
                    .HasForeignKey(AE => AE.TechnicianId)
                    .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(AE => AE.TechnicianId);
        }
    }

}
