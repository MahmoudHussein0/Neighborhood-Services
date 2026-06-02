using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;
using Neighborhood.Services.Domain.TechniciansAvailability;
using System;

namespace Neighborhood.Services.Application.TechnitianAvailability.Commands
{
    public class AddTechnicianAvailabilityCommandHandler : IRequestHandler<AddTechnicianAvailabilityCommand, int>
    {
        private readonly ITechnicianAvailabilityRepository _availabilityRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AddTechnicianAvailabilityCommandHandler(ITechnicianAvailabilityRepository availabilityRepo , IUnitOfWork unitOfWork)
        {
            _availabilityRepo = availabilityRepo;
           _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(AddTechnicianAvailabilityCommand request, CancellationToken cancellationToken)
        {

            if (request.EndTime <= request.StartTime)
            {
                throw new ValidationException(new Dictionary<string, string[]>
            {{ "TimeRange", new[] { "End Time must be greater than Start Time." }}});}


            if (await _availabilityRepo.HasOverlapAsync(
                    request.TechnicianId,
                    request.DayOfWeek,
                    request.StartTime,
                    request.EndTime))
            {
                    throw new ValidationException(new Dictionary<string, string[]>
                {{ "Overlap", new[] { "This availability overlaps with an existing time slot." } }});}


            var availability = new TechnicianAvailability()
            {
                TechnicianId = request.TechnicianId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime
            };
           
           await _availabilityRepo.AddAsync(availability);
           await _unitOfWork.SaveChangesAsync();

            return availability.Id;

        }
    }
}
