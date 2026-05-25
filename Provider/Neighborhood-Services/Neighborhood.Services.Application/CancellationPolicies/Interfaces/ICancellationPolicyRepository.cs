using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.CancellationPolicies;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.CancellationPolicies.Interfaces
{
    public interface ICancellationPolicyRepository: IGenericRepository<CancellationPolicy, int>
    {
        Task<CancellationPolicy?> GetPolicyAsync(int hoursBeforeBooking, CancellationPolicyTarget appliesTo);
    }
}
