using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Transactions;

namespace Neighborhood.Services.Domain.Wallets
{
    public class Wallet : BaseEntity<int>
    {
        public string UserId { get; set; }
        public decimal Balance { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Transaction> OutgoingTransactions { get; set; } = new HashSet<Transaction>();
        public ICollection<Transaction> IncomingTransactions { get; set; } = new HashSet<Transaction>();
    }
}