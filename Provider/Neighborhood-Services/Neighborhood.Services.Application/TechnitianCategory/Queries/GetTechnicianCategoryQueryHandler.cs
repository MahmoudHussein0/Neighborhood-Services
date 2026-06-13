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
            var lang = request.Lang.ToLower();  

            return (await _technicianCategoryRepo.GetByConditionAsync( 
                                TC =>
                                (!TC.IsDeleted)
                                &&
                                TC.TechnicianId == request.TechnicianId , "Category")
                )

                .Select(TC => new CategoryDto {
                  Id = TC.CategoryId,
                  Name =  lang == "en" ?  TC.Category.NameEn : TC.Category.NameAr,
                  Icon = TC.Category.Icon
            }).ToList();
        }
    }
}
