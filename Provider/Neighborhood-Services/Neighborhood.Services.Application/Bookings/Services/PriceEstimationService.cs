using Neighborhood.Services.Application.ProblemTypes.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Bookings.Services
{
    public class PriceEstimationService : IPriceEstimationService
    {
        private readonly IProblemTypeRepository _problemTypeRepository;

        public PriceEstimationService(IProblemTypeRepository problemTypeRepository)
        {
            _problemTypeRepository = problemTypeRepository;
        }

        public async Task<decimal> EstimateAsync(int problemTypeId)
        {
            var problemType = await _problemTypeRepository.GetByIdAsync(problemTypeId);
            return (problemType.MinPrice + problemType.MaxPrice) / 2;
        }
    }
}
