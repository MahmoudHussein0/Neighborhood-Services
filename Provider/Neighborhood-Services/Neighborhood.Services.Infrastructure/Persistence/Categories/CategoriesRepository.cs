using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Neighborhood.Services.Application.Categories.Interfaces;
using Neighborhood.Services.Domain.Categories;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using static Azure.Core.HttpHeader;

namespace Neighborhood.Services.Infrastructure.Persistence.Categories
{
    public class CategoriesRepository : GenericRepository<Category, int>, ICategoryRepository
    {
        public CategoriesRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<bool> IsNameExistsAsync(string? nameEn, string? nameAr, int? categoryId = null)
         => await _context.Categories.AnyAsync(c =>
                    !c.IsDeleted &&
                     c.Id != categoryId &&
                    (
                        (!string.IsNullOrWhiteSpace(nameEn) &&
                        c.NameEn.Trim().ToLower() == nameEn.Trim().ToLower())
                            ||
                        (!string.IsNullOrWhiteSpace(nameAr) &&
                        c.NameAr.Trim().ToLower() == nameAr.Trim().ToLower())
                    )
            );


    }
}
