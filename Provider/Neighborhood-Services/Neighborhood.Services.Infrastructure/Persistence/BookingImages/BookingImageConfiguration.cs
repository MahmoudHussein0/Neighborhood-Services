using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Neighborhood.Services.Domain.BookingImages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.BookingImages
{
    public class BookingImageConfiguration : IEntityTypeConfiguration<BookingImage>
    {
        public void Configure(EntityTypeBuilder<BookingImage> builder)
        {
            builder.HasKey(bi => bi.Id);

            builder.Property(bi => bi.Type)
            .HasConversion<string>();

            builder.HasOne(bi => bi.Booking)
            .WithMany(b => b.BookingImages)
            .HasForeignKey(bi => bi.BookingId)
            .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(bi => bi.UploadedByUser)
            .WithMany()
            .HasForeignKey(bi => bi.UploadedBy)
            .OnDelete(DeleteBehavior.NoAction);



        }
    }
}
