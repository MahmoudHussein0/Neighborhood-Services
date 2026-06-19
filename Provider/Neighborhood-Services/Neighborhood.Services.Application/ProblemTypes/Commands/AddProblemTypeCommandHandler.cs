using MediatR;
using Neighborhood.Services.Application.Categories.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ProblemTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Commands
{
    public class AddProblemTypeCommandHandler : IRequestHandler<AddProblemTypeCommand , int>
    {
        private readonly IProblemTypeRepository _problemTypeRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobScheduler _jobs;

        public AddProblemTypeCommandHandler( IProblemTypeRepository problemTypeRepo , ICategoryRepository categoryRepo , IUnitOfWork unitOfWork, IBackgroundJobScheduler jobs)
        {
           _problemTypeRepo = problemTypeRepo;
           _categoryRepo = categoryRepo;
           _unitOfWork = unitOfWork;
           _jobs = jobs;
        }

        public async  Task<int> Handle(AddProblemTypeCommand request, CancellationToken cancellationToken)
        {
            var category =  await  _categoryRepo.GetByIdAsync(request.CategoryId);

            if (category is null)
                throw new NotFoundException("Category" , request.CategoryId);

            if (await _problemTypeRepo.IsExistsAsync(request.NameEn , request.NameAr, request.CategoryId))
                throw new ValidationException("This problem already assigned to this category.");

            if (request.MinPrice <= 0)
                throw new ValidationException("MinPrice must be greater than zero.");

            if (request.MaxPrice <= 0)
                throw new ValidationException("MaxPrice must be greater than zero.");

            if (request.MinPrice >= request.MaxPrice)
                    throw new ValidationException("MinPrice must be less than MaxPrice.");

            var problemType = new ProblemType()
            {
                NameEn = request.NameEn , 
                NameAr = request.NameAr , 
                DescriptionEn = request.DescriptionEn ,
                DescriptionAr = request.DescriptionAr ,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice ,
                CategoryId = request.CategoryId
            };

            await _problemTypeRepo.AddAsync(problemType);
            await  _unitOfWork.SaveChangesAsync();

            // Embed this problem type into the RAG index off the request thread. Fail-open: a
            // queue hiccup must never undo a committed problem type.
            try { _jobs.EnqueueProblemTypeIndex(problemType.Id); } catch { /* RAG sync is best-effort; /reindex is the backstop */ }

            return problemType.Id;
        }
    }
}
