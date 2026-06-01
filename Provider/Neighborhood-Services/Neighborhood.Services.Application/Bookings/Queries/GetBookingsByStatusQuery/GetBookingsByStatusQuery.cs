using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingsByStatusQuery
{
    public class GetBookingsByStatusQuery : IRequest<IEnumerable<BookingSummaryDto>>
    {
        public BookingStatus Status { get; set; }
    }
}
