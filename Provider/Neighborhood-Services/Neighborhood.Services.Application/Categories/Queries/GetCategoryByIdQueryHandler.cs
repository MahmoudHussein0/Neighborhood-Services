using Mapster;
using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.Categories.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories.Queries
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDetailsDto>
    {
        private readonly ICategoryRepository _categoryRepo;

        public GetCategoryByIdQueryHandler( ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<CategoryDetailsDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
           var category = (await _categoryRepo.GetByConditionAsync(c => c.Id == request.Id, "ProblemTypes")).FirstOrDefault();

            if (category is null) throw new NotFoundException("Category" , request.Id);
           return   category.Adapt<CategoryDetailsDto>();
        }
    }
}
