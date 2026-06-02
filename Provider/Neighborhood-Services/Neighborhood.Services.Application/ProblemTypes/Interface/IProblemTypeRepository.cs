using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ProblemTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.ProblemTypes.Interface
{
    public  interface IProblemTypeRepository : IGenericRepository<ProblemType , int>
    {
        Task<bool> IsExistsAsync(string name , int categoryId );
    }
}
