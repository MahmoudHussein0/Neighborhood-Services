using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Commands
{
    public class DeleteProblemTypeCommandHandler : IRequestHandler<DeleteProblemTypeCommand, bool>
    {
        private readonly IProblemTypeRepository _problemTypeRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProblemTypeCommandHandler( IProblemTypeRepository problemTypeRepo , IUnitOfWork unitOfWork )
        {
            _problemTypeRepo = problemTypeRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteProblemTypeCommand request, CancellationToken cancellationToken)
        {
            var problemType = await _problemTypeRepo.GetByIdAsync(request.Id);
            
            if (problemType is null) throw new NotFoundException("ProblemType" , request.Id);

           await _problemTypeRepo.DeleteAsync(problemType.Id);
           return    await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
