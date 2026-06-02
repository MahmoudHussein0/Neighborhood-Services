using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Categories.Interfaces;
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

        public async Task<bool> IsNameExistsAsync(string name)
         => await  _context.Categories.AnyAsync(c => c.Name.ToLower().Contains(name.ToLower()));
    }
}
