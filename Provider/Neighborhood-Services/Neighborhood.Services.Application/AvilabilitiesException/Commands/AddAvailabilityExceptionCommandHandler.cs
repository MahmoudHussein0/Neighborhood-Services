using MediatR;
using Neighborhood.Services.Application.AvilabilitiesException.Interfaces;
using Neighborhood.Services.Application.Shared;
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

        public AddAvailabilityExceptionCommandHandler(IAvailabilityExceptionRepository exceptionRepo , IUnitOfWork unitOfWork)
        {
           _exceptionRepo = exceptionRepo;
           _unitOfWork = unitOfWork;
        }

        async Task<int> IRequestHandler<AddAvailabilityExceptionCommand, int>.Handle(AddAvailabilityExceptionCommand request, CancellationToken cancellationToken)
        {

            if( request.StartTime.HasValue || request.EndTime.HasValue)
            {
                if( !request.StartTime.HasValue  || !request.EndTime.HasValue)
                    throw new Exception("Both StartTime and EndTime must be provided.");

                if (request.EndTime <= request.StartTime)
                throw new Exception("End Time must be greater than Start Time");

            }


            if(await _exceptionRepo.IsDateExists(request.TechnicianId , request.Date ) )
                throw new Exception( "An exception already exists for this date.");


            var availiabilityException = new AvailabilityException()
            {
                TechnicianId = request.TechnicianId,
                Date = request.Date,
                Reason = request.Reason,
                StartTime = request.StartTime,
                EndTime = request.EndTime ,
                IsAvailable = request.IsAvailable
            };

             await  _exceptionRepo.AddAsync(availiabilityException);
             await _unitOfWork.SaveChangesAsync();

            return availiabilityException.Id;

        }
    }
}
