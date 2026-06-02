using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;

namespace Neighborhood.Services.Application.Bookings.Queries.GetMyBookingsQuery
{
    // Returns the authenticated user's bookings — works for both customers and technicians.
    public class GetMyBookingsQuery : IRequest<IEnumerable<BookingSummaryDto>>
    {
    }
}
