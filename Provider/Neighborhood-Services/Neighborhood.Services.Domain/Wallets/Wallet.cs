using Neighborhood.Services.Domain.Shared;
using System.Transactions;

namespace Neighborhood.Services.Domain.Wallets
{
    public class Wallet : BaseEntity
    {
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ICollection<Transaction> OutgoingTransactions { get; set; } = [];
        public ICollection<Transaction> IncomingTransactions { get; set; } = [];
    }
}