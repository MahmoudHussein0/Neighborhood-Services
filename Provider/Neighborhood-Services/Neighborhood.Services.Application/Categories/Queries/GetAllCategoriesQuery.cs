using MediatR;
using Neighborhood.Services.Application.Categories.DTOs;
using Neighborhood.Services.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories.Queries
{
    public  class GetAllCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>
    {
      
        public string? SearchTerm  { get; set; }

        public GetAllCategoriesQuery(string? searchTerm = null )
        {
            SearchTerm = searchTerm;
        }

    }
}
