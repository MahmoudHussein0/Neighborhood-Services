using Neighborhood.Services.Application.Shared;
using System.Transactions;
namespace Neighborhood.Services.Application.Transactions.Interfaces
{
    public interface ITransactionRepository : IGenericRepository<Transaction, int>
    {
    }
}