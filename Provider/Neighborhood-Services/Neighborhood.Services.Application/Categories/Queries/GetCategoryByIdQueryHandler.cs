using Mapster;
using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.Categories.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ProblemTypes.DTOs;

namespace Neighborhood.Services.Application.Categories.Queries
{
    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDetailsDto>
    {
        private readonly ICategoryRepository _categoryRepo;

        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<CategoryDetailsDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {

            var lang = request.Lang.ToLower();

            var category = (await _categoryRepo.GetByConditionAsync(c => (!c.IsDeleted) && c.Id == request.Id, "ProblemTypes")).FirstOrDefault();

            if (category is null) throw new NotFoundException("Category", request.Id);

            var problemTypes = category.ProblemTypes.Where( P =>!(P.IsDeleted)).Select(P => new ProblemTypeDto
            {
                Id = P.Id,
                Name = lang == "en" ? P.NameEn : P.NameAr,
                NameAr = P.NameAr,
                NameEn = P.NameEn,
                DescriptionAr = P.DescriptionAr,
                DescriptionEn =P.DescriptionEn,
                Description = lang == "en" ? P.DescriptionEn : P.DescriptionAr,
                MinPrice = P.MinPrice,
                MaxPrice = P.MaxPrice ,
                ImageUrl = P.ImageUrl??""
            }).ToList();

            var categoryDetailsDto = new CategoryDetailsDto()
            {
                Id = request.Id,
                Name = lang == "en" ? category.NameEn : category.NameAr,
                Icon = category.Icon,
                ProblemTypes = problemTypes
            };


            return categoryDetailsDto;
        }
    }
}
