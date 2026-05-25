using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Domain.AvilabilitiesException;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.AvilabilitiesException
{
    public class AvailabilityExceptionRepository : GenericRepository<AvailabilityException , int>
    {
        public AvailabilityExceptionRepository(ApplicationDbContext context):base(context)
        {}




        public override async  Task DeleteAsync(int id)
        {
           var availableExcep  =  await  GetByIdAsync(id);

            if(availableExcep is not null)
            {
                availableExcep.isDeleted = true;

              await  UpdateAsync(availableExcep);
            }
        }



    }
}
