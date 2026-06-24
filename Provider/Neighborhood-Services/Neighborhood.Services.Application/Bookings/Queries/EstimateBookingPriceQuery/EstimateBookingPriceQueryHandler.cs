using MediatR;
using Neighborhood.Services.Application.Bookings.Services;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.Application.Bookings.Queries.EstimateBookingPriceQuery
{
    public class EstimateBookingPriceQueryHandler : IRequestHandler<EstimateBookingPriceQuery, decimal>
    {
        private readonly IPriceEstimationService _priceEstimationService;
        private readonly IRegionResolver _regionResolver;

        public EstimateBookingPriceQueryHandler(
            IPriceEstimationService priceEstimationService,
            IRegionResolver regionResolver)
        {
            _priceEstimationService = priceEstimationService;
            _regionResolver = regionResolver;
        }

        public async Task<decimal> Handle(EstimateBookingPriceQuery request, CancellationToken cancellationToken)
        {
            // Resolve the region from the supplied coords (or explicit region) so the preview is
            // localized. Null => PriceEstimationService falls back to the general average.
            var region = await _regionResolver.ResolveAsync(
                request.Latitude, request.Longitude, regionOverride: request.Region,
                cancellationToken: cancellationToken);

            return await _priceEstimationService.EstimateAsync(request.ProblemTypeId, region);
        }
    }
}
