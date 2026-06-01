using Neighborhood.Services.Domain.Transactions;

namespace Neighborhood.Services.Application.Transactions.DTOs
{
    public class CreateTransactionDto
    {
        public int? FromWalletId { get; set; }
        public int? ToWalletId { get; set; }
        public int? PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}