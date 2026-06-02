using MediatR;
using Neighborhood.Services.Domain.BookingImages;

namespace Neighborhood.Services.Application.BookingImages.Commands
{
    public class UploadBookingImageCommand : IRequest<int>
    {
        public int BookingId { get; set; }
        // ImageUrl is the already-hosted URL returned by the frontend's direct upload to Cloudinary
        public string ImageUrl { get; set; } = string.Empty;
        public BookingImageType Type { get; set; }
        // UploadedBy is resolved from the authenticated user in the handler.
    }
}
