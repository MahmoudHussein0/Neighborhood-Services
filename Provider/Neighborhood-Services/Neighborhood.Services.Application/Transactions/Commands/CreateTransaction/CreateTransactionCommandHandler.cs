using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Transactions.DTOs;
using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Application.Transactions.Commands.CreateTransaction
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionResponseDto>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<TransactionResponseDto> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new InvalidOperationException("Transaction amount must be greater than zero.");

            if (request.FromWalletId is null && request.ToWalletId is null)
                throw new InvalidOperationException("A transaction must have a source or destination wallet.");

            if (request.FromWalletId == request.ToWalletId)
                throw new InvalidOperationException("Source and destination wallets must be different.");

            var transaction = new Transaction
            {
                FromWalletId = request.FromWalletId,
                ToWalletId = request.ToWalletId,
                PaymentMethodId = request.PaymentMethod,
                Amount = request.Amount,
                Type = request.Type,
                Status = TransactionStatus.Pending,
                Currency = "EGP"
            };

            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TransactionResponseDto
            {
                Id = transaction.Id,
                FromWalletId = transaction.FromWalletId,
                ToWalletId = transaction.ToWalletId,
                PaymentMethodId = transaction.PaymentMethodId,
                Amount = transaction.Amount,
                Fee = transaction.Fee,
                Currency = transaction.Currency,
                Type = transaction.Type,
                Status = transaction.Status,
                CreatedAt = transaction.CreatedAt
            };
        }
    }
}
