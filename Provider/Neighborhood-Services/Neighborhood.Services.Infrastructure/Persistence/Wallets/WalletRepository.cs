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

        public async Task CreditAsync(int walletId, decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Credit amount must be greater than zero.");

            var wallet = await GetByIdAsync(walletId)
                ?? throw new KeyNotFoundException($"Wallet with ID {walletId} not found.");

            wallet.Balance += amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(wallet);
        }

        public async Task DebitAsync(int walletId, decimal amount)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Debit amount must be greater than zero.");

            var wallet = await GetByIdAsync(walletId)
                ?? throw new KeyNotFoundException($"Wallet with ID {walletId} not found.");

            if (wallet.Balance < amount)
                throw new InvalidOperationException("Insufficient wallet balance.");

            wallet.Balance -= amount;
            wallet.UpdatedAt = DateTime.UtcNow;
            await UpdateAsync(wallet);
        }
    }
}
