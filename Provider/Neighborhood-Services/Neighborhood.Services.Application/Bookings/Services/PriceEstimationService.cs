using Neighborhood.Services.Application.HistoricalPrices.Interfaces;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Domain.HistoricalPrices;
using Neighborhood.Services.Domain.ProblemTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Bookings.Services
{
    public class PriceEstimationService : IPriceEstimationService
    {
        private readonly IProblemTypeRepository _problemTypeRepository;
        private readonly IHistoricalPriceRepository _historicalPriceRepository;

        public PriceEstimationService(IProblemTypeRepository problemTypeRepository, IHistoricalPriceRepository historicalPriceRepository)
        {
            _problemTypeRepository = problemTypeRepository;
            _historicalPriceRepository = historicalPriceRepository;
        }

        public async Task<decimal> EstimateAsync(int problemTypeId, string? region)
        {

            if (string.IsNullOrWhiteSpace(region))
            {
                var historicalPrice = (await _historicalPriceRepository.GetByProblemType(problemTypeId)).ToList();
                if (historicalPrice.Any())
                    return historicalPrice.Average(Hp => Hp.AveragePrice);
                var problemType = (await _problemTypeRepository.GetByIdAsync(problemTypeId));
                return (problemType.MinPrice + problemType.MaxPrice) / 2;
            }

            var historicalForRegion = await _historicalPriceRepository.GetByProblemTypeAndRegion(problemTypeId, region);
            if (historicalForRegion.Any())
                return historicalForRegion.Average(Hp => Hp.AveragePrice);

            var problemTypeByRegion = await _problemTypeRepository.GetByIdAsync(problemTypeId);
            return ((problemTypeByRegion.MinPrice + problemTypeByRegion.MaxPrice) / 2) * GetRegionMultiplier(region);
        }

        private decimal GetRegionMultiplier(string region)
            => region.ToLower() switch
            {
                "cairo" => 1.3m,
                "giza" => 1.2m,
                "alex" => 1.15m,
                "tanta" => 1.1m,
                "mahalla" => 1.0m,
                _ => 1.0m
            };

    }
}
