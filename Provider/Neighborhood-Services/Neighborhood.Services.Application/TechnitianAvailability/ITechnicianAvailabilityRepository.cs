using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.TechniciansAvailability;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.TechnitianAvailability
{
    public interface ITechnicianAvailabilityRepository : IGenericRepository<TechnicianAvailability , int>
    {
    }
}
