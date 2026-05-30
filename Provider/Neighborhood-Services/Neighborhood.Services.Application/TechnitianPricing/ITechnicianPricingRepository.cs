using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.TechniciansPricing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianPricing
{
    public interface ITechnicianPricingRepository : IGenericRepository<TechnicianPricing , int>
    {
    }
}
