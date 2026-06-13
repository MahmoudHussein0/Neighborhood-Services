using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianAvailability.Commands
{
    public class DeleteTechnicianAvailabilityCommandHandler : IRequestHandler<DeleteTechnicianAvailabilityCommand, bool>
    {

        private readonly ITechnicianAvailabilityRepository _availabilityRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTechnicianAvailabilityCommandHandler(ITechnicianAvailabilityRepository availabilityRepo, IUnitOfWork unitOfWork)
        {
            _availabilityRepo = availabilityRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(DeleteTechnicianAvailabilityCommand request, CancellationToken cancellationToken)
        {
           var availability = await _availabilityRepo.GetByIdAsync(request.Id);

            if(availability is null)
                throw new NotFoundException("Availability ");

           await _availabilityRepo.DeleteAsync(availability.Id);

            return await _unitOfWork.SaveChangesAsync() > 0;

        }
    }
}
