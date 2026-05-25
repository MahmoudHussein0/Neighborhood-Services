using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using System.Transactions;
namespace Neighborhood.Services.Infrastructure.Persistence.Transactions
{
    public class TransactionRepository : GenericRepository<Transaction, int>, ITransactionRepository
    {
        public TransactionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}