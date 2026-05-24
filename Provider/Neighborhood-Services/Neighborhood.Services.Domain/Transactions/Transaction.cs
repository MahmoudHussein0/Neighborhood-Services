using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Wallets;
namespace Neighborhood.Services.Domain.Transactions
{
    public class Transaction : BaseEntity
    {
        public int? FromWalletId { get; set; }
        public int? ToWalletId { get; set; }
        public int? PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; } = 0;
        public string Currency { get; set; } = "EGP";
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
        public int? OriginalTransactionId { get; set; }
        public Wallet? FromWallet { get; set; }
        public Wallet? ToWallet { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
    }
}