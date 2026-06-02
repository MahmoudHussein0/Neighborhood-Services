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
            return  (await _historicalRepository.GetByConditionAsync(HP => HP.ProblemTypeId == request.ProblemTypeId, "ProblemType"))
                .OrderByDescending(HP => HP.CreatedAt)
                .Select(HP => new HistoricalPricingDto
                {
                    ProblemName = HP.ProblemType.Name,
                    ProblemDescription = HP.ProblemType.Description,
                    AveragePrice = HP.AveragePrice,
                    MaterialCost = HP.MaterialCost,
                }).ToList();
        }
    }
}
