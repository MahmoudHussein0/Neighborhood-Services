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

        public async Task<IEnumerable<PromoCodeUsage>> GetByUserIdAsync(string userId)
        => await GetByConditionAsync(u => u.UserId == userId, includeProperties: "PromoCode");

        public async Task<bool> HasUserUsedPromoAsync(string userId, int promoCodeId)
        {
            var res = await GetByConditionAsync(pu =>
                pu.UserId == userId &&
                pu.PromoCodeId == promoCodeId);  
            return res.Any();
        }
    }
}