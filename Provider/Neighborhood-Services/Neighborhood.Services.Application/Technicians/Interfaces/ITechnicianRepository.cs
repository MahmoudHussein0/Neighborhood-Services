using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Technicians;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Technicians.Interfaces
{
    public interface ITechnicianRepository : IGenericRepository<Technician, int>
    {
    }
}
