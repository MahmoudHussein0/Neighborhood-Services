using Neighborhood.Services.Application.PromoCodes.Interface;
using Neighborhood.Services.Domain.PromoCodes;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.PromoCodes
{
    public class PromoCodeUsageRepository : GenericRepository<PromoCodeUsage, int>, IPromoCodeUsageRepository
    {
        public PromoCodeUsageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}