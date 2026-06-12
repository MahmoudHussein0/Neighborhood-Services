using MediatR;
using Neighborhood.Services.Application.RecurringBookings.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.RecurringBookings;

namespace Neighborhood.Services.Application.RecurringBookings.Queries.GetMyRecurringBookingsQuery
{
    // Returns the authenticated user's recurring bookings (customer or technician), paged + optional filter/search.
    public class GetMyRecurringBookingsQuery : IRequest<PagedResult<RecurringBookingDto>>
    {
        public RecurringBookingStatus? Status { get; set; }   // optional filter
        public string? Search { get; set; }                   // matches address or exact id
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
