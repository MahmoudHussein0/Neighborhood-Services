using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianCategory.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianCategory.Queries
{
    public class GetTechnicianCategoryQueryHandler : IRequestHandler<GetTechnicianCategoryQuery, IReadOnlyList<CategoryDto>>
    {

        private readonly ITechnicianCategoryRepository _technicianCategoryRepo;

        public GetTechnicianCategoryQueryHandler(ITechnicianCategoryRepository technicianCategoryRepo)
        {
            _technicianCategoryRepo = technicianCategoryRepo;
        }
        public async Task<IReadOnlyList<CategoryDto>> Handle(GetTechnicianCategoryQuery request, CancellationToken cancellationToken)
        {
            return (await _technicianCategoryRepo.GetByConditionAsync(TC => TC.TechnicianId == request.TechnicianId , "Category"))
                .Select(TC => new CategoryDto {
                  Name = TC.Category.Name ,
                  Icon = TC.Category.Icon
            }).ToList();
        }
    }
}
