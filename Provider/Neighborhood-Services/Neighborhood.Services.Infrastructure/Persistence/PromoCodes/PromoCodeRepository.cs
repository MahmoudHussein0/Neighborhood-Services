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
    }
}