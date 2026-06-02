using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingsByTechnicianQuery
{
    public class GetBookingsByTechnicianQuery : IRequest<IEnumerable<BookingSummaryDto>>
    {
        public int TechnicianId { get; set; }
    }
}
