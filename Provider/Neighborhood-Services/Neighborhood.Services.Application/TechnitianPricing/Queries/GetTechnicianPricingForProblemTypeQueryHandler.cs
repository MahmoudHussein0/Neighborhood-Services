using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;
using Neighborhood.Services.Application.TechnitianPricing.Interface;

namespace Neighborhood.Services.Application.TechnitianPricing.Queries
{
    public class GetTechnicianPricingForProblemTypeQueryHandler : IRequestHandler<GetTechnicianPricingForProblemTypeQuery, IReadOnlyList<TechnicianPricingDto>>
    {
        private readonly ITechnicianPricingRepository _technicianPricingRepo;

        public GetTechnicianPricingForProblemTypeQueryHandler(ITechnicianPricingRepository technicianPricingRepo)
        {
            _technicianPricingRepo = technicianPricingRepo;
        }

        public async Task<IReadOnlyList<TechnicianPricingDto>> Handle(GetTechnicianPricingForProblemTypeQuery request, CancellationToken cancellationToken)
        {
            var pricing = await _technicianPricingRepo.GetByConditionAsync(TP => TP.TechnicianId == request.TechnicianId, "ProblemType");

            if (pricing == null || !pricing.Any())
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {{ "TechnicianId", new[] { "No pricing found for this technician." } }});
            }

            return pricing
                .OrderBy(TP => TP.MinPrice)
                .Select(TP => new TechnicianPricingDto
                {
                    ProblemTypeName = TP.ProblemType.Name,
                    TechPriceMaxPrice = TP.MaxPrice,
                    TechPriceMinPrice = TP.MinPrice
                })
                .ToList();
        }
    }
}
