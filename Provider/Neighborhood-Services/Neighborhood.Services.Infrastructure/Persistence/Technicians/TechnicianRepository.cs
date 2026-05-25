using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Infrastructure.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Infrastructure.Persistence.Context;

namespace Neighborhood.Services.Infrastructure.Persistence.Technicians
{
    public class TechnicianRepository : GenericRepository<Technician, int>, ITechnicianRepository
    {
        public TechnicianRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
