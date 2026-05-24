using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.BookingImages
{
    public class BookingImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public BookingImageType Type { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime UploadedAt { get; set; }

        // Foreign Keys
        public int BookingId { get; set; }
        public int UploadedBy { get; set; }
    }
}
