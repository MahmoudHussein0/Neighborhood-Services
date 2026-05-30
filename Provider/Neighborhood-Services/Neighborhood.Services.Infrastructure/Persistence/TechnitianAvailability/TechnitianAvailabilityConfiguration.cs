using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.TechniciansAvailability;


namespace Neighborhood.Services.Infrastructure.Persistence.TechnitianAvailability
{
    public class TechnitianAvailabilityConfiguration : IEntityTypeConfiguration<TechnicianAvailability>
    {
        public void Configure(EntityTypeBuilder<TechnicianAvailability> builder)
        {

            builder.Property(TA => TA.StartTime)
              .IsRequired();


            builder.Property(TA => TA.EndTime)
                 .IsRequired();

            builder.Property(TA => TA.IsDeleted)
                   .HasDefaultValue(false);

            builder.HasQueryFilter(TA => !TA.IsDeleted);

            builder.Property(TA => TA.DayOfWeek)
                   .HasMaxLength(20)
                   .HasConversion(
                  Day => Day.ToString(),
                  Day => Enum.Parse<DayOfWeek>(Day));

            builder.HasOne(TA => TA.Technician)
                   .WithMany(t=>t.TechnicianAvailabilities)
                   .HasForeignKey(TA => TA.TechnicianId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(TA => TA.TechnicianId);
        }
    }
}
