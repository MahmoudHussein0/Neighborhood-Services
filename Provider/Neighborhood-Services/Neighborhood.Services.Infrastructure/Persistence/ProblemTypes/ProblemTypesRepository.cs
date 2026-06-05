using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.ProblemTypes.Interface;
using Neighborhood.Services.Domain.ProblemTypes;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.ProblemTypes
{
    public class ProblemTypesRepository : GenericRepository<ProblemType, int>, IProblemTypeRepository
    {
        public ProblemTypesRepository(ApplicationDbContext context) : base(context)
        { }

        public async Task<bool> IsExistsAsync(string nameAr , string nameEn , int categoryId )
        => await _context.ProblemTypes.AnyAsync( P => (  EF.Functions.Like( P.NameEn.ToLower() , nameEn.ToLower())  || EF.Functions.Like(P.NameAr.ToLower(), nameAr.ToLower()))  && P.CategoryId == categoryId);

    }
}
