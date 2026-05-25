using Neighborhood.Services.Domain.TechnicionsPricing;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.TechnitianPricing
{
    public class TechnicianPricingRepository :GenericRepository<TechnicianPricing , int>
    {
        public TechnicianPricingRepository(ApplicationDbContext context):base(context)
        {}


        public override async  Task DeleteAsync(int id)
        {
         var techPricing =  await GetByIdAsync(id);
            if(techPricing is not null)
            {
                techPricing.IsDeleted = true;
              await  UpdateAsync(techPricing);
            }
        }
    }
}
