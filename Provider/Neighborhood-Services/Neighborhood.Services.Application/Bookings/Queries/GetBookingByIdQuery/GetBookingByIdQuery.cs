using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingByIdQuery
{
    public class GetBookingByIdQuery : IRequest<BookingDetailsDto>
    {
        public int BookingId { get; set; }
    }
}
