using MediatR;
using Neighborhood.Services.Application.Bookings.Services;

namespace Neighborhood.Services.Application.Bookings.Queries.EstimateBookingPriceQuery
{
    public class EstimateBookingPriceQueryHandler : IRequestHandler<EstimateBookingPriceQuery, decimal>
    {
        private readonly IPriceEstimationService _priceEstimationService;

        public EstimateBookingPriceQueryHandler(IPriceEstimationService priceEstimationService)
        {
            _priceEstimationService = priceEstimationService;
        }

        public Task<decimal> Handle(EstimateBookingPriceQuery request, CancellationToken cancellationToken)
            => _priceEstimationService.EstimateAsync(request.ProblemTypeId, request.Region);
    }
}
