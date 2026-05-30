using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.PromoCodes;
namespace Neighborhood.Services.Application.PromoCodes.Interface
{
    public interface IPromoCodeRepository : IGenericRepository<PromoCode, int>
    {
        Task<PromoCode?> GetByCodeAsync (string code);
        Task<bool> IsValidAsync (string code);
        Task IncrementUsageAsync(int promoCodeId);
    }
}