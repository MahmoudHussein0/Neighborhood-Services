using MediatR;
using Neighborhood.Services.Application.AvilabilitiesException.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.AvilabilitiesException;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AvilabilitiesException.Commands
{
    public class AddAvailabilityExceptionCommandHandler : IRequestHandler<AddAvailabilityExceptionCommand, int>
    {
        private readonly IAvailabilityExceptionRepository _exceptionRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianRepository _technicianRepo;

        public AddAvailabilityExceptionCommandHandler(IAvailabilityExceptionRepository exceptionRepo , IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ITechnicianRepository technicianRepo)
        {
           _exceptionRepo = exceptionRepo;
           _unitOfWork = unitOfWork;
           _currentUserService = currentUserService;
            _technicianRepo = technicianRepo;
        }

        async Task<int> IRequestHandler<AddAvailabilityExceptionCommand, int>.Handle(AddAvailabilityExceptionCommand request, CancellationToken cancellationToken)
        {
            string? userId = _currentUserService.UserId;


            if (userId is null)
                throw new UnauthorizedException("User not autherized");


            var technician = await _technicianRepo.GetByUserIdAsync(userId);

            if (technician is null)
                throw new ForbiddenException("User is not a technician");


            if ( request.StartTime.HasValue || request.EndTime.HasValue)
            {
                if( !request.StartTime.HasValue  || !request.EndTime.HasValue)
                throw new ValidationException("Both StartTime and EndTime must be provided.");

                if (request.EndTime <= request.StartTime)
                    throw new ValidationException("End Time must be greater than Start Time.");
            }

            if(await _exceptionRepo.IsDateExists( technician.Id , request.Date ) )
            throw new ValidationException("An exception already exists for this date.");


            var availiabilityException = new AvailabilityException()
            {
                TechnicianId = technician.Id,
                Date = request.Date,
                Reason = request.Reason,
                StartTime = request.StartTime,
                EndTime = request.EndTime ,
                IsAvailable = request.IsAvailable
            };

             await  _exceptionRepo.AddAsync(availiabilityException);
             await _unitOfWork.SaveChangesAsync(cancellationToken);

            return availiabilityException.Id;

        }
    }
}
