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

        public UpdateProblemTypeCommandHandler(IProblemTypeRepository problemTypeRepo, IUnitOfWork unitOfWork)
        {
            _problemTypeRepo = problemTypeRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateProblemTypeDto> Handle(UpdateProblemTypeCommand request, CancellationToken cancellationToken)
        {
            var problemType = await _problemTypeRepo.GetByIdAsync(request.Id);

            if (problemType is null) throw new NotFoundException("ProblemType", request.Id);


            if (request.MinPrice <= 0)
                throw new ValidationException(new Dictionary<string, string[]>
                {{ "MinPrice", new[] { "MinPrice must be greater than zero." }}});

            if (request.MaxPrice <= 0)
                throw new ValidationException(new Dictionary<string, string[]>
                {{"MaxPrice", new[] { "MaxPrice must be greater than zero." }}});

            if (request.MinPrice >= request.MaxPrice)
                throw new ValidationException(new Dictionary<string, string[]>
                {{ "PriceRange", new[] { "MinPrice must be less than MaxPrice." } } });


            problemType.Description = request.Description;
            problemType.MinPrice = request.MinPrice;
            problemType.MaxPrice = request.MaxPrice;


            await _problemTypeRepo.UpdateAsync(problemType);
            await _unitOfWork.SaveChangesAsync();

            return problemType.Adapt<UpdateProblemTypeDto>();
        }
    }
}
