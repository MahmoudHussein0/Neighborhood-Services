using Microsoft.EntityFrameworkCore;
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
            var normalizedCode = code.Trim();
            var result = await GetByConditionAsync(p => p.Code == normalizedCode && !p.IsDeleted);
            return result.FirstOrDefault();
        }

        public async Task<bool> TryIncrementUsageAsync(int promoCodeId)
        {
            var affectedRows = await _context.PromoCodes
                .Where(p => p.Id == promoCodeId
                    && p.IsActive
                    && !p.IsDeleted
                    && p.ExpiresAt > DateTime.UtcNow
                    && p.UsedCount < p.MaxUses)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(p => p.UsedCount, p => p.UsedCount + 1));

            return affectedRows == 1;
        }

        public async Task<bool> IsValidAsync(string code)
        {
            var normalizedCode = code.Trim();
            var result = await GetByConditionAsync(p =>
                p.Code == normalizedCode &&
                p.IsActive &&
                !p.IsDeleted &&
                p.ExpiresAt > DateTime.UtcNow &&
                p.UsedCount < p.MaxUses);

            return result.Any();
        }
    }
}
