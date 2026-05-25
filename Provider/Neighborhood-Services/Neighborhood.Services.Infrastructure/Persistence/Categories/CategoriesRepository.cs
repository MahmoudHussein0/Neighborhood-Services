using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.Categories
{
    public class CategoriesRepository : GenericRepository<Category , int>
    {
        public CategoriesRepository( ApplicationDbContext context):base(context)
        {}
        
        public override async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);

            if(category is not null)
            {
                category.IsDeleted = true;

               await UpdateAsync(category);
            }
        }

    }
}
