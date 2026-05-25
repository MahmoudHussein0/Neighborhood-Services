using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Shared;

namespace Neighborhood.Services.Domain.BookingImages
{
    public class BookingImage : BaseEntity<int>
    {
        public string ImageUrl { get; set; } = string.Empty;
        public BookingImageType Type { get; set; }
        public DateTime UploadedAt { get; set; }

        public int BookingId { get; set; }
        public string UploadedBy { get; set; }

        public Booking Booking { get; set; } = null!;
        public ApplicationUser UploadedByUser { get; set; } = null!;
    }
}
