using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Categories.Interfaces
{
    public  interface ICategoryRepository : IGenericRepository<Category  , int>
    {


        Task<bool> IsNameExistsAsync(string nameEn , string nameAr, int? categoryId = null);
       
    }
}
