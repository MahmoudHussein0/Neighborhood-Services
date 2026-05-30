using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.TechnicianPhotos;

namespace Neighborhood.Services.Infrastructure.Persistence.TechnicianPhotos
{
    public class TechnicianPhotoConfiguration : IEntityTypeConfiguration<TechnicianPhoto>
    {
        public void Configure(EntityTypeBuilder<TechnicianPhoto> builder)
        {
            builder.HasKey(tp => tp.Id);

            builder.Property(tp => tp.PhotoUrl)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(tp => tp.Caption)
                .HasMaxLength(500);

            builder.Property(tp => tp.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(tp => tp.ApplicationUserId)
                .HasMaxLength(450)
                .IsRequired();

            builder.HasOne(tp => tp.Technician)
                .WithMany(t => t.TechnicianPhotos)
                .HasForeignKey(tp => tp.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
