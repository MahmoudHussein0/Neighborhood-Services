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
        private readonly IBackgroundJobScheduler _jobs;

        public DeleteProblemTypeCommandHandler( IProblemTypeRepository problemTypeRepo , IUnitOfWork unitOfWork, IBackgroundJobScheduler jobs )
        {
            _problemTypeRepo = problemTypeRepo;
            _unitOfWork = unitOfWork;
            _jobs = jobs;
        }

        public async Task<bool> Handle(DeleteProblemTypeCommand request, CancellationToken cancellationToken)
        {
            var problemType = await _problemTypeRepo.GetByIdAsync(request.Id);

            if (problemType is null) throw new NotFoundException("ProblemType" , request.Id);

            await _problemTypeRepo.DeleteAsync(problemType.Id);
            var deleted = await _unitOfWork.SaveChangesAsync() > 0;

            // Drop this problem type's vectors from the RAG index off the request thread.
            // Fail-open: a queue hiccup must never undo a committed delete.
            if (deleted)
                try { _jobs.EnqueueProblemTypeRemoval(problemType.Id); } catch { /* RAG sync is best-effort; /reindex is the backstop */ }

            return deleted;
        }
    }
}
