using MediatR;

namespace Neighborhood.Services.Application.Bookings.Queries.EstimateBookingPriceQuery
{
    // Optional, on-demand price estimate for a problem type (history/rule based, optionally region-adjusted).
    public class EstimateBookingPriceQuery : IRequest<decimal>
    {
        public int ProblemTypeId { get; set; }
        public string? Region { get; set; }

        // Optional GPS coords. When Region isn't given, the handler resolves it from these so the
        // estimate is localized (matches what the customer sees when actually creating the booking).
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
