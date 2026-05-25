using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.BookingImages
{
    public class BookingImage :BaseEntity<int>
    {
        public string ImageUrl { get; set; } = string.Empty;
        public BookingImageType Type { get; set; }
        public DateTime UploadedAt { get; set; }

        // Foreign Keys
        public int BookingId { get; set; }
        public string UploadedBy { get; set; }

        // Nav
        public Booking Booking { get; set; } = null!;
        public ApplicationUser UploadedByUser { get; set; } = null!;
    }
}
