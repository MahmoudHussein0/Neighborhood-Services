using Neighborhood.Services.Application.CancellationPolicies.Interfaces;
using Neighborhood.Services.Domain.CancellationPolicies;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.CancellationPolicies
{
    public class CancellationPolicyRepository : GenericRepository<CancellationPolicy, int>, ICancellationPolicyRepository
    {
        public CancellationPolicyRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<CancellationPolicy?> GetPolicyAsync(int hoursBeforeBooking, CancellationPolicyTarget appliesTo)
        {
            return await _context.CancellationPolicies
                .FirstOrDefaultAsync(cp => cp.HoursBeforeBooking <= hoursBeforeBooking
                    && cp.AppliesTo == appliesTo);
        }
    }
}
