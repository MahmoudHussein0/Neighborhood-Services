using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using Neighborhood.Services.Domain.Transactions;
using Neighborhood.Services.Application.Shared;
using System.Linq.Expressions;
namespace Neighborhood.Services.Infrastructure.Persistence.Transactions
{
    public class TransactionRepository : GenericRepository<Transaction, int>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Transaction>> GetByTypeAsync(TransactionType type)
        => await GetByConditionAsync(t => t.Type == type);

        public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId)
        => await GetByConditionAsync(t => t.FromWalletId == walletId || t.ToWalletId == walletId);
    }
}