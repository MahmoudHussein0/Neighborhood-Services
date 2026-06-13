using MediatR;
using Neighborhood.Services.Application.AvilabilitiesException.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AvilabilitiesException.Commands
{
    public class DeleteAvailabilityExceptionCommandHandler : IRequestHandler<DeleteAvailabilityExceptionCommand, bool>
    {


        private readonly IAvailabilityExceptionRepository _exceptionRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAvailabilityExceptionCommandHandler(IAvailabilityExceptionRepository exceptionRepo, IUnitOfWork unitOfWork)
        {
            _exceptionRepo = exceptionRepo;
            _unitOfWork = unitOfWork;
        }


        public async Task<bool> Handle(DeleteAvailabilityExceptionCommand request, CancellationToken cancellationToken)
        {
            var exception = await _exceptionRepo.GetByIdAsync( request.Id );

            if (exception is null)
                throw new NotFoundException("AvabilityException " , exception.Id);

           await _exceptionRepo.DeleteAsync(exception.Id);
           return await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
