using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.TechnicianCategories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianCategory.Interface
{
    public interface  ITechnicianCategoryRepository : IGenericRepository<TechnicianCategory , int>
    {
        Task<bool> IsExists(int technicianId , int categoryId);

    }
}
