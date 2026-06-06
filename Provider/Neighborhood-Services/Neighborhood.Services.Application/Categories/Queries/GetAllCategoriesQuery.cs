using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;

namespace Neighborhood.Services.Application.Categories.Queries
{
    public  class GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>
    {
        public string Lang { get; set; }
        public string? SearchTerm  { get; set; }


        public GetAllCategoriesQuery(string lang, string? searchTerm)
        {
            Lang = lang;
            SearchTerm = searchTerm;
        }
    }
}
