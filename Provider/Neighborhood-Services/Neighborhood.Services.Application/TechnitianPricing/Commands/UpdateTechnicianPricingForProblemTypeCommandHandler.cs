using Mapster;
using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.HistoricalPrices.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianPricing.DTOs;
using Neighborhood.Services.Application.TechnitianPricing.Interface;
using Neighborhood.Services.Domain.HistoricalPrices;
using Neighborhood.Services.Domain.ProblemTypes;

namespace Neighborhood.Services.Application.TechnitianPricing.Commands
{
    public class UpdateTechnicianPricingForProblemTypeCommandHandler : IRequestHandler<UpdateTechnicianPricingForProblemTypeCommand , UpdatePricingDTO>
    {
        private readonly ITechnicianPricingRepository _pricingRepo;
        private readonly IHistoricalPriceRepository _historicalRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTechnicianPricingForProblemTypeCommandHandler(ITechnicianPricingRepository pricingRepo, IHistoricalPriceRepository historicalRepo , IUnitOfWork unitOfWork)
        {
            _pricingRepo = pricingRepo;
            _historicalRepo = historicalRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdatePricingDTO> Handle(UpdateTechnicianPricingForProblemTypeCommand request, CancellationToken cancellationToken)
        {
            var pricing =  await _pricingRepo.GetByIdAsync(request.Id);

            if (pricing is null)
                throw new NotFoundException("Pricing", request.Id);

            if (request.MinPrice <= 0)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                {{ "MinPrice", new[] { "MinPrice must be greater than zero." }}});}


            if (request.MaxPrice <= 0)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                    {{ "MaxPrice", new[] { "MaxPrice must be greater than zero." }}});}



            if (request.MinPrice >= request.MaxPrice)
            {
                throw new ValidationException(new Dictionary<string, string[]>
                     {{ "PriceRange", new[] { "MinPrice must be less than MaxPrice." }}}); }


            pricing.MinPrice = request.MinPrice;
            pricing.MaxPrice = request.MaxPrice;
            pricing.UpdatedAt = DateTime.UtcNow;

            await _pricingRepo.UpdateAsync(pricing);


            var historical = new HistoricalPrice()
            {
                ProblemTypeId = pricing.ProblemTypeId,
                Region = "",
                AveragePrice = (request.MinPrice + request.MaxPrice) / 2,
                MaterialCost = 0
            };

            await _historicalRepo.AddAsync(historical);


            await _unitOfWork.SaveChangesAsync();

           return  pricing.Adapt<UpdatePricingDTO>();

        }
    }
}
