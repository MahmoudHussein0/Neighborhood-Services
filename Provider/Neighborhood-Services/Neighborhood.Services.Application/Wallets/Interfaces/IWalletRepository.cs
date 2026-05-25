using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Wallets;
namespace Neighborhood.Services.Application.Wallets.Interfaces
{
    public interface IWalletRepository : IGenericRepository<Wallet, int>
    {
    }
}