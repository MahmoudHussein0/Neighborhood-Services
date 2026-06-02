using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.TechniciansPricing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianPricing.Interface
{
    public interface ITechnicianPricingRepository : IGenericRepository<TechnicianPricing , int>
    {

        Task<bool> IsExistsAsync(int technicianId, int problemTypeId);



    }
}
