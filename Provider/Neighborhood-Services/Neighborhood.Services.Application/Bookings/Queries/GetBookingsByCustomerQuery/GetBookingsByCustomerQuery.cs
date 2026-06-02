using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingsByCustomerQuery
{
    public class GetBookingsByCustomerQuery : IRequest<IEnumerable<BookingSummaryDto>>
    {
        public int CustomerId { get; set; }
    }
}
