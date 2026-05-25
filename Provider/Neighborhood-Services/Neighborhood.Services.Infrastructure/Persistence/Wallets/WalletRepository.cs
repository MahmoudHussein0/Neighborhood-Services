using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Wallets;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.Wallets
{
    public class WalletRepository : GenericRepository<Wallet, int>, IWalletRepository
    {
        public WalletRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}