using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Application.Transactions.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction, int>
    {
        Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId);
        Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType type);
    }
}