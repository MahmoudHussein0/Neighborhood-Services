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

        public AddProblemTypeCommandHandler( IProblemTypeRepository problemTypeRepo , ICategoryRepository categoryRepo , IUnitOfWork unitOfWork)
        {
           _problemTypeRepo = problemTypeRepo;
           _categoryRepo = categoryRepo;
           _unitOfWork = unitOfWork;
        }

        public async  Task<int> Handle(AddProblemTypeCommand request, CancellationToken cancellationToken)
        {
            var category =  await  _categoryRepo.GetByIdAsync(request.CategoryId);

            if (category is null)
                throw new NotFoundException("Category" , request.CategoryId);

            if (await _problemTypeRepo.IsExistsAsync(request.Name, request.CategoryId))
                throw new ValidationException(new Dictionary<string, string[]>
                {{ "Name", new[] { "This problem already assigned to this category." }}});

            if (request.MinPrice <= 0)
                throw new ValidationException(new Dictionary<string, string[]>
                {{ "MinPrice", new[] { "MinPrice must be greater than zero." }}});

            if (request.MaxPrice <= 0)
                throw new ValidationException(new Dictionary<string, string[]>
                {{ "MaxPrice", new[] { "MaxPrice must be greater than zero." }}});

            if (request.MinPrice >= request.MaxPrice)
                    throw new ValidationException(new Dictionary<string, string[]>
                 {{ "PriceRange", new[] { "MinPrice must be less than MaxPrice." }}});

            var problemType = new ProblemType()
            {
                Name = request.Name , 
                Description = request.Description ,
                MinPrice = request.MinPrice,
                MaxPrice = request.MaxPrice ,
                CategoryId = request.CategoryId
            };

            await _problemTypeRepo.AddAsync(problemType);
            await  _unitOfWork.SaveChangesAsync(); 
            return problemType.Id;
        }
    }
}
