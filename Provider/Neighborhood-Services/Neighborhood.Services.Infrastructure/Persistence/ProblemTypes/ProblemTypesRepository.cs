using Neighborhood.Services.Application.ProblemTypes;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.ProblemTypes
{
    public  class ProblemTypesRepository : GenericRepository<ProblemType , int>  , IProblemTypeRepository
    {

        public ProblemTypesRepository(ApplicationDbContext context):base(context)
        {}



       


    }
}
