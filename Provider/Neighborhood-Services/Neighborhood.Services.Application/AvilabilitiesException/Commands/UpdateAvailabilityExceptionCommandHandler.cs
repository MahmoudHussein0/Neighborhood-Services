using Mapster;
using MediatR;
using Neighborhood.Services.Application.AvilabilitiesException.DTOs;
using Neighborhood.Services.Application.AvilabilitiesException.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AvilabilitiesException.Commands
{
    public class UpdateAvailabilityExceptionCommandHandler : IRequestHandler<UpdateAvailabilityExceptionCommand, AvailiabilityExceptionDTO>
    {

        private readonly IAvailabilityExceptionRepository _exceptionRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAvailabilityExceptionCommandHandler(IAvailabilityExceptionRepository exceptionRepo, IUnitOfWork unitOfWork)
        {
            _exceptionRepo = exceptionRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<AvailiabilityExceptionDTO> Handle(UpdateAvailabilityExceptionCommand request, CancellationToken cancellationToken)
        {

          var exception =   await _exceptionRepo.GetByIdAsync(request.Id);

            if (exception is null)
                throw new NotFoundException("AvabilityException" , request.Id);


            if (request.StartTime.HasValue || request.EndTime.HasValue)
            {
                if (!request.StartTime.HasValue || !request.EndTime.HasValue)
                    throw new ValidationException("Both StartTime and EndTime must be provided.");



                if (request.EndTime <= request.StartTime)
                    throw new ValidationException("End Time must be greater than Start Time.");
            }


            if (await _exceptionRepo.IsDateExists(exception.TechnicianId, request.Date, request.Id))
            {
                throw new ValidationException("An exception already exists for this date.");}

            exception.Date = request.Date;
            exception.StartTime = request.StartTime;
            exception.EndTime = request.EndTime;
            exception.IsAvailable = request.IsAvailable;
            exception.Reason = request.Reason;

            await _exceptionRepo.UpdateAsync(exception);

            await _unitOfWork.SaveChangesAsync();


            return exception.Adapt<AvailiabilityExceptionDTO>();

        }
    }
}
