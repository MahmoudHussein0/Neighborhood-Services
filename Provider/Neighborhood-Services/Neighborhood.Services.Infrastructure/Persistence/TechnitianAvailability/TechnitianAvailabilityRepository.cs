using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Domain.TechnicionsAvailability;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.TechnitianAvailability
{
    public  class TechnitianAvailabilityRepository : GenericRepository<TechnicianAvailability ,  int>
    {


        public TechnitianAvailabilityRepository(ApplicationDbContext context):base(context)
        {}


        public override async  Task DeleteAsync(int id)
        {
            var techAvailability = await GetByIdAsync(id);
        
            if(techAvailability is not null)
            {
                techAvailability.IsDeleted = true;
               await UpdateAsync(techAvailability);
            }
        }



    }
}
