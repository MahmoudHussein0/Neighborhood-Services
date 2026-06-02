using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.TechniciansAvailability;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianAvailability.Interfaces
{
    public interface ITechnicianAvailabilityRepository : IGenericRepository<TechnicianAvailability , int>
    {
        Task<bool> HasOverlapAsync(int technicianId, DayOfWeek dayOfWeek, TimeOnly startDate, TimeOnly endDate , int? techAvailiabilityId = null);
    }
}
