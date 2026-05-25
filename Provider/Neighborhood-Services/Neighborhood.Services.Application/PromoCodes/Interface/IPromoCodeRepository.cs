using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.PromoCodes;
namespace Neighborhood.Services.Application.PromoCodes.Interface
{
    public interface IPromoCodeRepository : IGenericRepository<PromoCode, int>
    {
    }
}