using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.ProblemTypes
{
    public  class ProblemTypesRepository : GenericRepository<ProblemType , int>
    {

        public ProblemTypesRepository(ApplicationDbContext context):base(context)
        {}



        public override async  Task DeleteAsync(int id)
        {
            var problemType = await GetByIdAsync(id);
            if( problemType is not null)
            {
                problemType.IsDeleted = true;

               await UpdateAsync(problemType);
            }
        }




    }
}
