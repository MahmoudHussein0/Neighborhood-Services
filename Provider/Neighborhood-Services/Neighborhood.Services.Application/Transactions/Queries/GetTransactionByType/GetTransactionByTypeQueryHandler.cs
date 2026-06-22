using MediatR;
using Neighborhood.Services.Application.Transactions.DTOs;
using Neighborhood.Services.Application.Transactions.Interfaces;
namespace Neighborhood.Services.Application.Transactions.Queries.GetTransactionByType
{
    public class GetTransactionByTypeQueryHandler : IRequestHandler<GetTransactionByTypeQuery, IEnumerable<TransactionResponseDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        public GetTransactionByTypeQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public async Task<IEnumerable<TransactionResponseDto>> Handle(GetTransactionByTypeQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _transactionRepository.GetByTypeAsync(request.Type);
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
                CreatedAt = DateTime.SpecifyKind(t.CreatedAt, DateTimeKind.Utc)
            });
        }
    }
}