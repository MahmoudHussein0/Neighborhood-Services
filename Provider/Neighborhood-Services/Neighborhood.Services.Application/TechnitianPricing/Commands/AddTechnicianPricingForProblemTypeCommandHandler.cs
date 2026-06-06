using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.HistoricalPrices.Interfaces;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.TechnitianPricing.Interface;
using Neighborhood.Services.Domain.HistoricalPrices;
using Neighborhood.Services.Domain.TechniciansPricing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianPricing.Commands
{
    public class AddTechnicianPricingForProblemTypeCommandHandler : IRequestHandler<AddTechnicianPricingForProblemTypeCommand, int>
    {

        private readonly ITechnicianPricingRepository _technicianPricingRepo;
        private readonly IHistoricalPriceRepository _historicalRepository;
        private readonly ITechnicianRepository _technicianRepo;
        private readonly IProblemTypeRepository _problemTypeRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddTechnicianPricingForProblemTypeCommandHandler(ITechnicianPricingRepository technicianPricingRepo ,IHistoricalPriceRepository historicalRepository , ITechnicianRepository technicianRepo , IProblemTypeRepository problemTypeRepo , IUnitOfWork unitOfWork)
        {
            _technicianPricingRepo = technicianPricingRepo;
            _historicalRepository = historicalRepository;
            _technicianRepo = technicianRepo;
           _problemTypeRepo = problemTypeRepo;
            _unitOfWork = unitOfWork;
        }


        public async Task<int> Handle(AddTechnicianPricingForProblemTypeCommand request, CancellationToken cancellationToken)
        {
            var technician = await _technicianRepo.GetByIdAsync(request.TechnicianId);
            var problemType = await _problemTypeRepo.GetByIdAsync(request.ProblemTypeId);

             if( technician is null )
                throw new NotFoundException("Technician" , request.TechnicianId);


             if( problemType is null)
                throw new NotFoundException("Problem" , request.ProblemTypeId);


            if (await _technicianPricingRepo.IsExistsAsync(request.TechnicianId, request.ProblemTypeId))
            {
                throw new ValidationException("Technician already has pricing for this problem.");}



            if (request.MinPrice <= 0)
            {
                throw new ValidationException("MinPrice must be greater than zero.");}


            if (request.MaxPrice <= 0)
            {
                throw new ValidationException("MaxPrice must be greater than zero.");}



            if (request.MinPrice >= request.MaxPrice)
            {
                throw new ValidationException("MinPrice must be less than MaxPrice.");}

            var techPricing = new TechnicianPricing()
            {
                TechnicianId = request.TechnicianId ,
                ProblemTypeId = request.ProblemTypeId, 
                MinPrice = request.MinPrice ,
                MaxPrice = request.MaxPrice ,
            };

            await _technicianPricingRepo.AddAsync(techPricing);


            var historical = new HistoricalPrice()
            {
                ProblemTypeId = problemType.Id,
                Region = "",
                AveragePrice = (request.MinPrice + request.MaxPrice ) / 2,
                MaterialCost = 0
            };

           await _historicalRepository.AddAsync(historical);
            
           await _unitOfWork.SaveChangesAsync();




            return techPricing.Id;
        }
    }
}
