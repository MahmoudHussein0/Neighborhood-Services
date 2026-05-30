using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Wallets;
namespace Neighborhood.Services.Application.Wallets.Interfaces
{
    public interface IWalletRepository : IGenericRepository<Wallet, int>
    {
        Task<Wallet?> GetByUserIdAsync(string userId);
        Task UpdateBalanceAsync(int walletId, decimal newBalance);
    }
}