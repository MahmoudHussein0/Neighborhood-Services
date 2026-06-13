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
        private readonly ICurrentUserService _currentUserService;

        public AddTechnicianPricingForProblemTypeCommandHandler(ITechnicianPricingRepository technicianPricingRepo, IHistoricalPriceRepository historicalRepository, ITechnicianRepository technicianRepo, IProblemTypeRepository problemTypeRepo, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _technicianPricingRepo = technicianPricingRepo;
            _historicalRepository = historicalRepository;
            _technicianRepo = technicianRepo;
            _problemTypeRepo = problemTypeRepo;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }


        public async Task<int> Handle(AddTechnicianPricingForProblemTypeCommand request, CancellationToken cancellationToken)
        {

            string? userId = _currentUserService.UserId;

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedException("User is not authenticated");

            var technician = await _technicianRepo.GetByUserIdAsync(userId);

            if (technician is null)
                throw new ForbiddenException("User is not a technician");

            var technicianPricing = (await _technicianPricingRepo.GetByConditionAsync(TP => TP.TechnicianId == technician.Id && TP.ProblemTypeId == request.ProblemTypeId)).FirstOrDefault();

            if (request.TechMinPrice <= 0)
            {
                throw new ValidationException("MinPrice must be greater than zero.");
            }

            if (request.TechMaxPrice <= 0)
            {
                throw new ValidationException("MaxPrice must be greater than zero.");
            }

            if (request.TechMinPrice >= request.TechMaxPrice)
            {
                throw new ValidationException("MinPrice must be less than MaxPrice.");
            }

            if (technicianPricing is not null && !technicianPricing.IsDeleted)
                throw new ValidationException("pricing already existing.");



            if (technicianPricing is not null && technicianPricing.IsDeleted)
            {
                technicianPricing.IsDeleted = false;
                technicianPricing.MinPrice = request.TechMinPrice;
                technicianPricing.MaxPrice = request.TechMaxPrice;
                await _technicianPricingRepo.UpdateAsync(technicianPricing);
            }
            else
            {
                var problemType = await _problemTypeRepo.GetByIdAsync(request.ProblemTypeId);

                if (problemType is null)
                    throw new NotFoundException("Problem", request.ProblemTypeId);





                technicianPricing = new TechnicianPricing()
                {
                    TechnicianId = technician.Id,
                    ProblemTypeId = request.ProblemTypeId,
                    MinPrice = request.TechMinPrice,
                    MaxPrice = request.TechMaxPrice,
                };

                await _technicianPricingRepo.AddAsync(technicianPricing);

            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return technicianPricing.Id;
        }
    }
}
