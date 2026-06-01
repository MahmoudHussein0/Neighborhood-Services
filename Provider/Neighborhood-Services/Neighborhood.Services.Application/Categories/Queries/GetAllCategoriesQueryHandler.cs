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
          var categories =( await  _categoryRepo.GetByConditionAsync(
                c =>
                !c.IsDeleted &&
                (string.IsNullOrWhiteSpace(request.SearchTerm) || c.Name.ToLower().Contains(request.SearchTerm.ToLower()))
                )).OrderByDescending(c => c.CreatedAt);

           return  categories.Adapt<IReadOnlyList<CategoryDto>>();
        }
    }
}
