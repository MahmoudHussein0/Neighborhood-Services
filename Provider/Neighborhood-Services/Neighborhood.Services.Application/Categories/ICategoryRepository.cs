using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories
{
    public  interface ICategoryRepository : IGenericRepository<Category  , int>
    {
    }
}
