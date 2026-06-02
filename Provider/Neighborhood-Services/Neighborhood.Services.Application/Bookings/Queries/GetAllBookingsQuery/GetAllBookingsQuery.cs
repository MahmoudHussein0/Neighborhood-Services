using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;

namespace Neighborhood.Services.Application.Bookings.Queries.GetAllBookingsQuery
{
    // Admin: list every booking.
    public class GetAllBookingsQuery : IRequest<IEnumerable<BookingSummaryDto>>
    {
    }
}
