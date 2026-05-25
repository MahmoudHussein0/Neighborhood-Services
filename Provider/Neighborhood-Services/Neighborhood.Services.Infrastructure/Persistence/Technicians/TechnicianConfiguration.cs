using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.ApplicationUser;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Infrastructure.Persistence.Technicians
{
    public class TechnicianConfiguration : IEntityTypeConfiguration<Technician>
    {
        public void Configure(EntityTypeBuilder<Technician> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.NationalId)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(t => t.Experience)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(t => t.Rating)
                .HasPrecision(3, 2);

            builder.Property(t => t.VerificationStatus)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(t => t.IsAvailable)
                .HasDefaultValue(true);

            builder.Property(t => t.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true);

            builder.Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(t => t.ApplicationUserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.HasIndex(t => t.NationalId)
                .IsUnique();

            builder.HasOne<ApplicationUser>()
                .WithOne(u => u.Technician)
                .HasForeignKey<Technician>(t => t.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
