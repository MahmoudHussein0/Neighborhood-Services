using MediatR;
using Neighborhood.Services.Application.HistoricalPrices.DTOs;
using Neighborhood.Services.Application.HistoricalPrices.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.HistoricalPrices.Queries
{
    public class GetHistoricalPricesForProblemTypeQueryHandler : IRequestHandler<GetHistoricalPricesForProblemTypeQuery, IReadOnlyList<HistoricalPricingDto>>
    {
        private readonly IHistoricalPriceRepository _historicalRepository;

        public GetHistoricalPricesForProblemTypeQueryHandler(IHistoricalPriceRepository historicalRepository)
        {
           _historicalRepository = historicalRepository;
        }
        public async Task<IReadOnlyList<HistoricalPricingDto>> Handle(GetHistoricalPricesForProblemTypeQuery request, CancellationToken cancellationToken)
        {


            var lang = request.Lang ?? "en";
            return  (await _historicalRepository.GetByConditionAsync(HP => HP.ProblemTypeId == request.ProblemTypeId, "ProblemType"))
                .OrderByDescending(HP => HP.CreatedAt)
                .Select(HP => new HistoricalPricingDto
                {
                    ProblemName =  lang == "en" ?  HP.ProblemType.NameEn : HP.ProblemType.NameAr,
                    ProblemDescription =  lang == "en" ?  HP.ProblemType.DescriptionEn : HP.ProblemType.DescriptionAr,
                    AveragePrice = HP.AveragePrice,
                    MaterialCost = HP.MaterialCost,
                }).ToList();
        }
    }
}
