using Mapster;
using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Application.Categories.Interfaces;

namespace Neighborhood.Services.Application.Categories.Queries
{
    public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IReadOnlyList<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepo;

        public GetAllCategoriesQueryHandler(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IReadOnlyList<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var lang = request.Lang.ToLower();
            var categories =( await  _categoryRepo.GetByConditionAsync(
                c => (!c.IsDeleted)  &&
                ( lang == "en" ? (string.IsNullOrEmpty(request.SearchTerm)) ||  (c.NameEn.ToLower().Contains(request.SearchTerm)) : (string.IsNullOrEmpty(request.SearchTerm)) || (c.NameAr.ToLower().Contains(request.SearchTerm) ))
                )).OrderByDescending(c => c.CreatedAt);


            var categoriesDto = categories.Select(c => new CategoryDto()
            {
                Name = lang == "en" ? c.NameEn : c.NameAr,
                Icon = c.Icon,
                Id = c.Id
             }).ToList();

           return categoriesDto;
        }
    }
}
