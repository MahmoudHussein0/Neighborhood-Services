using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Application.Transactions.DTOs
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public int? FromWalletId { get; set; }
        public int? ToWalletId { get; set; }
        public int? PaymentMethodId { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string Currency { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public TransactionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}