using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.PromoCodes;
namespace Neighborhood.Services.Application.PromoCodes.Interface
{
    public interface IPromoCodeUsageRepository : IGenericRepository<PromoCodeUsage, int>
    {
        Task<bool> HasUserUsedPromoAsync(string userId, int promoCodeId);
        Task<IEnumerable<PromoCodeUsage>> GetByUserIdAsync(string userId);
    }
}