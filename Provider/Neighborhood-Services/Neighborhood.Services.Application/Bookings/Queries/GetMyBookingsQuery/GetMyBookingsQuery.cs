using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Enums;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Queries.GetMyBookingsQuery
{
    // Returns the authenticated user's bookings — works for both customers and technicians.
    public class GetMyBookingsQuery : IRequest<PagedResult<MyBookingSummaryDto>>
    {
        public BookingStatus? Status { get; set; }   // optional filter
        public string? Search { get; set; }           // matches description, address, or exact id
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public BookingSortBy Sort { get; set; } = BookingSortBy.NewestCreated;
    }
}
