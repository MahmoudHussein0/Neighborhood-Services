using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;
using Neighborhood.Services.Domain.TechniciansAvailability;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.TechnitianAvailability
{
    public  class TechnitianAvailabilityRepository : GenericRepository<TechnicianAvailability ,  int>  , ITechnicianAvailabilityRepository
    {


        public TechnitianAvailabilityRepository(ApplicationDbContext context):base(context)
        {}

        public Task<bool> HasOverlapAsync(int technicianId, DayOfWeek dayOfWeek, TimeOnly startDate, TimeOnly endDate , int? techAvailiabilityId = null )
        {

          return   _context.TechnicianAvailabilities.AnyAsync(
                             TA => ( TA.TechnicianId == technicianId  &&
                                     TA.DayOfWeek == dayOfWeek &&
                                     startDate < TA.EndTime &&
                                     endDate > TA.StartTime && 
                                    (!techAvailiabilityId.HasValue || techAvailiabilityId.Value != TA.Id)
                                     ));

        }
    }
}
