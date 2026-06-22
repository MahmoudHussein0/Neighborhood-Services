using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Enums;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingsForStaffQuery
{
    // Staff-only oversight: every booking, paged, with names + optional status filter / search.
    public class GetBookingsForStaffQuery : IRequest<PagedResult<StaffBookingDto>>
    {
        public BookingStatus? Status { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public BookingSortBy Sort { get; set; } = BookingSortBy.NewestCreated;
    }
}
