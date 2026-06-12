using MediatR;

namespace Neighborhood.Services.Application.Bookings.Queries.EstimateBookingPriceQuery
{
    // Optional, on-demand price estimate for a problem type (history/rule based, optionally region-adjusted).
    public class EstimateBookingPriceQuery : IRequest<decimal>
    {
        public int ProblemTypeId { get; set; }
        public string? Region { get; set; }
    }
}
