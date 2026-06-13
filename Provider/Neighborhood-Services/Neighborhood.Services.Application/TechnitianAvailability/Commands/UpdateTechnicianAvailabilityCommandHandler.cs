using Mapster;
using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianAvailability.DTOs;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianAvailability.Commands
{
    public class UpdateTechnicianAvailabilityCommandHandler : IRequestHandler<UpdateTechnicianAvailabilityCommand, TechnicianAvailabilityDTO>
    {

        private readonly ITechnicianAvailabilityRepository _availabilityRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTechnicianAvailabilityCommandHandler(ITechnicianAvailabilityRepository availabilityRepo, IUnitOfWork unitOfWork)
        {
            _availabilityRepo = availabilityRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<TechnicianAvailabilityDTO> Handle(UpdateTechnicianAvailabilityCommand request, CancellationToken cancellationToken)
        {

           var techAvailability = await _availabilityRepo.GetByIdAsync(request.Id);

            if(techAvailability is null)
                throw new NotFoundException("Availability" , techAvailability.Id);

            if (request.EndTime <= request.StartTime)
            {
                throw new ValidationException("End Time must be greater than Start Time.");}


            if (await _availabilityRepo.HasOverlapAsync(techAvailability.TechnicianId, request.DayOfWeek, request.StartTime, request.EndTime, techAvailability.Id))
            {
                throw new ValidationException("This availability overlaps with an existing time slot.");}



            techAvailability.DayOfWeek = request.DayOfWeek;
            techAvailability.StartTime = request.StartTime;
            techAvailability.EndTime = request.EndTime;


            await _availabilityRepo.UpdateAsync(techAvailability);
            await _unitOfWork.SaveChangesAsync();

            return techAvailability.Adapt<TechnicianAvailabilityDTO>();
        }
    }
}
