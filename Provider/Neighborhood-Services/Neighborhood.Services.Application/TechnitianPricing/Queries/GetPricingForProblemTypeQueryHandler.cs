using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;
using Neighborhood.Services.Application.TechnitianPricing.Interface;

namespace Neighborhood.Services.Application.TechnitianPricing.Queries
{
    public class GetPricingForProblemTypeQueryHandler : IRequestHandler<GetPricingForProblemTypeQuery, IReadOnlyList<TechnicianPricingDto>>
    {
        private readonly ITechnicianPricingRepository _technicianPricingRepo;

        public GetPricingForProblemTypeQueryHandler(ITechnicianPricingRepository technicianPricingRepo)
        {
            _technicianPricingRepo = technicianPricingRepo;
        }

        public async Task<IReadOnlyList<TechnicianPricingDto>> Handle(GetPricingForProblemTypeQuery request, CancellationToken cancellationToken)
        {


            var lang = request.Lang.ToLower() ?? "en";
            var pricing = await _technicianPricingRepo.GetByConditionAsync(TP => (!TP.IsDeleted)  &&  TP.TechnicianId == request.TechnicianId, "ProblemType");

            if (pricing == null || !pricing.Any())
            {
                throw new ValidationException("No pricing found for this technician.");
            }

            return pricing
                .OrderBy(TP => TP.MinPrice)
                .Select(TP => new TechnicianPricingDto
                {
                    ProblemTypeName = lang == "en" ?  TP.ProblemType.NameEn : TP.ProblemType.NameAr,
                    TechPriceMaxPrice = TP.MaxPrice,
                    TechPriceMinPrice = TP.MinPrice
                })
                .ToList();
        }
    }
}
