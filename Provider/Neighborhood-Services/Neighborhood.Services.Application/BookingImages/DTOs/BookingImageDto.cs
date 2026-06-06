using Neighborhood.Services.Domain.BookingImages;

namespace Neighborhood.Services.Application.BookingImages.DTOs
{
    public class BookingImageDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public BookingImageType Type { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
