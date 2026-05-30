using System;
using System.Collections.Generic;
using System.Text;
using Neighborhood.Services.Application.AvilabilitiesException;
using Neighborhood.Services.Domain.AvilabilitiesException;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.AvilabilitiesException
{
    public class AvailabilityExceptionRepository : GenericRepository<AvailabilityException , int>  , IAvailabilityExceptionRepository
    {
        public AvailabilityExceptionRepository(ApplicationDbContext context):base(context)
        {}






    }
}
