using MediatR;
using Neighborhood.Services.Application.AvilabilitiesException.Commands;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;
using Neighborhood.Services.Domain.TechniciansAvailability;
using System;

namespace Neighborhood.Services.Application.TechnitianAvailability.Commands
{
    public class AddTechnicianAvailabilityCommandHandler : IRequestHandler<AddTechnicianAvailabilityCommand, int>
    {
        private readonly ITechnicianAvailabilityRepository _availabilityRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianRepository _technicianRepo;

        public AddTechnicianAvailabilityCommandHandler(ITechnicianAvailabilityRepository availabilityRepo , IUnitOfWork unitOfWork  , ICurrentUserService currentUserService  , ITechnicianRepository  technicianRepo)
        {
            _availabilityRepo = availabilityRepo;
           _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _technicianRepo = technicianRepo;
        }
        public async Task<int> Handle(AddTechnicianAvailabilityCommand request, CancellationToken cancellationToken)
        {

           string? userId =  _currentUserService.UserId;


            if (userId is null)
                throw new UnauthorizedException("User not autherized");


            var technician =await  _technicianRepo.GetByUserIdAsync(userId);

            if (technician is null)
                throw new ForbiddenException("User is not a technician");

            if (request.EndTime <= request.StartTime)
            {
                throw new ValidationException("End Time must be greater than Start Time.");}


            if (await _availabilityRepo.HasOverlapAsync(
                     technician.Id,
                    request.DayOfWeek,
                    request.StartTime,
                    request.EndTime))
            {
                    throw new ValidationException("This availability overlaps with an existing time slot.");}


            var availability = new TechnicianAvailability()
            {
                TechnicianId = technician.Id,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };
           
           await _availabilityRepo.AddAsync(availability);
           await _unitOfWork.SaveChangesAsync(cancellationToken);

            return availability.Id;

        }
    }
}
