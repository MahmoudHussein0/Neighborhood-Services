using Neighborhood.Services.Application.Categories;
using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Categories
{
    public class CategoriesRepository : GenericRepository<Category , int>  , ICategoryRepository
    {
        public CategoriesRepository( ApplicationDbContext context):base(context)
        {}
        
       
    }
}
