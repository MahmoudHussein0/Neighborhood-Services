using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.TechnitianCategory.Interface;
using Neighborhood.Services.Domain.TechnicianCategories;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.TechnicianCategories
{
    public class TechnicianCategoryRepository : GenericRepository<TechnicianCategory , int> , ITechnicianCategoryRepository
    {
        public TechnicianCategoryRepository( ApplicationDbContext context):base(context)
        {}

       
    }
}
