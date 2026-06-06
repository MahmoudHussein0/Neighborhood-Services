using MediatR;
using Neighborhood.Services.Application.Transactions.DTOs;
using Neighborhood.Services.Application.Transactions.Interfaces;
namespace Neighborhood.Services.Application.Transactions.Queries.GetTransactionsByWalletId
{
    public class GetTransactionsByWalletIdQueryHandler : IRequestHandler<GetTransactionsByWalletIdQuery, IEnumerable<TransactionResponseDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        public GetTransactionsByWalletIdQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public async Task<IEnumerable<TransactionResponseDto>> Handle(GetTransactionsByWalletIdQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _transactionRepository.GetByWalletIdAsync(request.WalletId);
            return transactions.Select(t => new TransactionResponseDto
            {
                Id = t.Id,
                FromWalletId = t.FromWalletId,
                ToWalletId = t.ToWalletId,
                PaymentMethodId = t.PaymentMethodId,
                Amount = t.Amount,
                Fee = t.Fee,
                Currency = t.Currency,
                Type = t.Type,
                Status = t.Status,
                CreatedAt = t.CreatedAt
            });
        }
    }
}
