using MediatR;
using Neighborhood.Services.Application.BookingImages.DTOs;

namespace Neighborhood.Services.Application.BookingImages.Queries.GetBookingImagesQuery
{
    public class GetBookingImagesQuery : IRequest<IEnumerable<BookingImageDto>>
    {
        public int BookingId { get; set; }
    }
}
