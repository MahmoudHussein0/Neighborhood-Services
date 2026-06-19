using Mapster;
using MediatR;
using MediatR.Pipeline;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ProblemTypes.DTOs;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Commands
{
    public class UpdateProblemTypeCommandHandler : IRequestHandler<UpdateProblemTypeCommand, UpdateProblemTypeDto>
    {
        private readonly IProblemTypeRepository _problemTypeRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobScheduler _jobs;

        public UpdateProblemTypeCommandHandler(IProblemTypeRepository problemTypeRepo, IUnitOfWork unitOfWork, IBackgroundJobScheduler jobs)
        {
            _problemTypeRepo = problemTypeRepo;
            _unitOfWork = unitOfWork;
            _jobs = jobs;
        }

        public async Task<UpdateProblemTypeDto> Handle(UpdateProblemTypeCommand request, CancellationToken cancellationToken)
        {
            var problemType = await _problemTypeRepo.GetByIdAsync(request.Id);

            if (problemType is null) throw new NotFoundException("ProblemType", request.Id);


            if (request.MinPrice <= 0)
                throw new ValidationException("MinPrice must be greater than zero.");

            if (request.MaxPrice <= 0)
                throw new ValidationException("MaxPrice must be greater than zero.");

            if (request.MinPrice >= request.MaxPrice)
                throw new ValidationException("MinPrice must be less than MaxPrice.");



            if (!string.IsNullOrWhiteSpace(request.NameEn))
                problemType.NameEn = request.NameEn;


            if (!string.IsNullOrWhiteSpace(request.NameAr))
                problemType.NameAr = request.NameAr;

            if (!string.IsNullOrWhiteSpace(request.DescriptionEn))
                    problemType.DescriptionEn = request.DescriptionEn;

            if (!string.IsNullOrWhiteSpace(request.DescriptionAr))
                problemType.DescriptionAr = request.DescriptionAr;


            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
                problemType.ImageUrl = request.ImageUrl;


            problemType.MinPrice = request.MinPrice;
            problemType.MaxPrice = request.MaxPrice;


            await _problemTypeRepo.UpdateAsync(problemType);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Re-embed this problem type's vectors off the request thread. Fail-open: a queue
            // hiccup must never undo a committed update.
            try { _jobs.EnqueueProblemTypeIndex(problemType.Id); } catch { /* RAG sync is best-effort; /reindex is the backstop */ }

            var problemTypeDto = new UpdateProblemTypeDto()
            {
                Description = !string.IsNullOrWhiteSpace(request.DescriptionEn) ? request.DescriptionEn : request.DescriptionAr,
                ImageUrl = request.ImageUrl,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice
            };

            return problemTypeDto;
        }
    }
}
