using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.Reviews;

namespace Neighborhood.Services.Infrastructure.Persistence.Reviews.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .UseIdentityColumn();

            builder.Property(r => r.BookingId)
                .IsRequired();

            builder.Property(r => r.ReviewerId)
                .IsRequired();

            builder.Property(r => r.RevieweeId)
                .IsRequired();

            builder.Property(r => r.Rating)
                .IsRequired()
                .HasAnnotation("Range", new[] { 1, 5 }); // 1 to 5 stars

            builder.Property(r => r.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(r => r.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            // One Review → One ReviewAnalysis
            builder.HasOne(r => r.Analysis)
                .WithOne(a => a.Review)
                .HasForeignKey<ReviewAnalysis>(a => a.ReviewId)
                .OnDelete(DeleteBehavior.NoAction);

            // Soft delete filter — IsDeleted reviews are invisible by default
            //builder.HasQueryFilter(r => !r.IsDeleted);
            //This way the unique constraint is on (BookingId + ReviewerId) together — 
            //meaning the same person can't review the same booking twice,
            //but both the customer and the technician can each leave their own review. That's exactly the business rule you need
            builder.HasIndex(r => new { r.BookingId, r.ReviewerId })
                .IsUnique()
                .HasDatabaseName("IX_Reviews_BookingId_ReviewerId");

            builder.HasIndex(r => r.ReviewerId)
                .HasDatabaseName("IX_Reviews_ReviewerId");

            builder.HasIndex(r => r.RevieweeId)
                .HasDatabaseName("IX_Reviews_RevieweeId");

            builder.HasIndex(r => r.Status)
                .HasDatabaseName("IX_Reviews_Status");
        }
    }
}