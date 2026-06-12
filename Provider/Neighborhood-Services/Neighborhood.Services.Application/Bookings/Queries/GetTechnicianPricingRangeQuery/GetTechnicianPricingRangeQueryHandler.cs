using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.TechnitianPricing.Interface;

namespace Neighborhood.Services.Application.Bookings.Queries.GetTechnicianPricingRangeQuery
{
    public class GetTechnicianPricingRangeQueryHandler : IRequestHandler<GetTechnicianPricingRangeQuery, TechnicianPricingRangeDto?>
    {
        private readonly ITechnicianPricingRepository _technicianPricingRepository;

        public GetTechnicianPricingRangeQueryHandler(ITechnicianPricingRepository technicianPricingRepository)
        {
            _technicianPricingRepository = technicianPricingRepository;
        }

        public async Task<TechnicianPricingRangeDto?> Handle(GetTechnicianPricingRangeQuery request, CancellationToken cancellationToken)
        {
            var rows = await _technicianPricingRepository.GetByConditionAsync(
                tp => !tp.IsDeleted
                    && tp.TechnicianId == request.TechnicianId
                    && tp.ProblemTypeId == request.ProblemTypeId);

            var pricing = rows.FirstOrDefault();
            if (pricing is null)
                return null;

            return new TechnicianPricingRangeDto
            {
                TechnicianId = pricing.TechnicianId,
                ProblemTypeId = pricing.ProblemTypeId,
                MinPrice = pricing.MinPrice,
                MaxPrice = pricing.MaxPrice
            };
        }
    }
}
