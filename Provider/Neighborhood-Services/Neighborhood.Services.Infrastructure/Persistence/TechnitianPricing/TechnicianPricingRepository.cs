using Neighborhood.Services.Application.TechnitianPricing;
using Neighborhood.Services.Domain.TechniciansPricing;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.TechnitianPricing
{
    public class TechnicianPricingRepository : GenericRepository<TechnicianPricing, int>  , ITechnicianPricingRepository
    {
        public TechnicianPricingRepository(ApplicationDbContext context):base(context)
        {}


       
    }
}
