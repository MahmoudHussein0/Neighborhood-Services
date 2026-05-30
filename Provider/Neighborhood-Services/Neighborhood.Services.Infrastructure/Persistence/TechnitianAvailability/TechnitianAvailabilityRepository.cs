
using Neighborhood.Services.Application.TechnitianAvailability;
using Neighborhood.Services.Domain.TechniciansAvailability;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.TechnitianAvailability
{
    public  class TechnitianAvailabilityRepository : GenericRepository<TechnicianAvailability ,  int>  , ITechnicianAvailabilityRepository
    {


        public TechnitianAvailabilityRepository(ApplicationDbContext context):base(context)
        {}





    }
}
