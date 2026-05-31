using MediatR;
using Neighborhood.Services.Application.Transactions.DTOs;
using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Application.Transactions.Commands.CreateTransaction
{
    public class CreateTransactionCommand : IRequest<TransactionResponseDto>
    {
        public int? FromWalletId { get; set; }
        public int? ToWalletId { get; set; }
        public int? PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}