using MediatR;
using Neighborhood.Services.Application.BookingImages.DTOs;
using Neighborhood.Services.Domain.BookingImages;

namespace Neighborhood.Services.Application.BookingImages.Queries.GetBookingImagesByTypeQuery
{
    public class GetBookingImagesByTypeQuery : IRequest<IEnumerable<BookingImageDto>>
    {
        public int BookingId { get; set; }
        public BookingImageType Type { get; set; }
    }
}
