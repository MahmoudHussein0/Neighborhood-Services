using Neighborhood.Services.Application.PromoCodes.Interface;
using Neighborhood.Services.Domain.PromoCodes;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.PromoCodes
{
    public class PromoCodeRepository : GenericRepository<PromoCode, int>, IPromoCodeRepository
    {
        public PromoCodeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PromoCode?> GetByCodeAsync(string code)
        {
            var result = await GetByConditionAsync(p => p.Code == code && !p.IsDeleted);
            return result.FirstOrDefault();
        }

        public async Task IncrementUsageAsync(int promoCodeId)
        {
            var promo = await GetByIdAsync(promoCodeId);
            if (promo is null) return;

            promo.UsedCount++;
            await UpdateAsync(promo);
        }

        public async Task<bool> IsValidAsync(string code)
        {
            var result = await GetByConditionAsync(p =>
                p.Code == code &&
                p.IsActive &&
                !p.IsDeleted &&
                p.ExpiresAt > DateTime.UtcNow &&
                p.UsedCount < p.MaxUses);

            return result.Any();
        }
    }
}