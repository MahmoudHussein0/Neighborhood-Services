using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.AvilabilitiesException.Interfaces;
using Neighborhood.Services.Domain.AvilabilitiesException;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.AvilabilitiesException
{
    public class AvailabilityExceptionRepository : GenericRepository<AvailabilityException , int>  , IAvailabilityExceptionRepository
    {
        public AvailabilityExceptionRepository(ApplicationDbContext context):base(context)
        {}

        public async Task<bool> IsDateExists(int technicianId, DateOnly date , int? exceptionId = null)
        => await _context.AvailabilityExceptions.AnyAsync(AE => AE.TechnicianId == technicianId && AE.Date == date && (!exceptionId.HasValue ||   AE.Id != exceptionId));

     }
}
