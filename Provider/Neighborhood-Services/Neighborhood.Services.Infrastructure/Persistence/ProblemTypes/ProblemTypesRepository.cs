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

        public async Task<bool> IsExistsAsync(string nameEn , string nameAr , int categoryId)
        {
            var normalizedNameEn = nameEn?.Trim().ToLower();
            var normalizedNameAr = nameAr?.Trim().ToLower();

            return await _context.ProblemTypes.AnyAsync(P =>
                !P.IsDeleted &&
                P.CategoryId == categoryId &&
                (
                    (!string.IsNullOrWhiteSpace(normalizedNameEn) &&
                     P.NameEn.Trim().ToLower() == normalizedNameEn)

                    ||

                    (!string.IsNullOrWhiteSpace(normalizedNameAr) &&
                     P.NameAr.Trim().ToLower() == normalizedNameAr)
                )
            );
        }
    }
}
