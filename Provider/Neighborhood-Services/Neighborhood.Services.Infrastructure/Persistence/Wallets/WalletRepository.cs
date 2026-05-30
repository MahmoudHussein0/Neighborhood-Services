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

        public async Task<Wallet?> GetByUserIdAsync(string userId)
        {
            var res = await GetByConditionAsync(w => w.UserId == userId);
            return res.FirstOrDefault();
        }

        public async Task UpdateBalanceAsync(int walletId, decimal newBalance)
        {
            var wallet = await GetByIdAsync(walletId);
            if (wallet is null) return;
            wallet.Balance = newBalance;
            wallet.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(wallet);
        }
    }
}