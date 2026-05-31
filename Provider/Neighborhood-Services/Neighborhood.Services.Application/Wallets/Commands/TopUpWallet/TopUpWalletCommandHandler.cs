using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Application.Wallets.DTOs;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Application.Wallets.Commands.TopUpWallet
{
    public class TopUpWalletCommandHandler : IRequestHandler<TopUpWalletCommand, WalletResponseDto>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;
        public TopUpWalletCommandHandler(IWalletRepository walletRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<WalletResponseDto> Handle(TopUpWalletCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new InvalidOperationException("Top-up amount must be greater than zero.");

            if (request.PaymentMethodId <= 0)
                throw new InvalidOperationException("A valid payment method is required.");

            var wallet = await _walletRepository.GetByIdAsync(request.WalletId)
            ?? throw new KeyNotFoundException($"Wallet with ID {request.WalletId} not found.");

            var transaction = new Transaction
            {
                ToWalletId = wallet.Id,
                PaymentMethodId = request.PaymentMethodId,
                Amount = request.Amount,
                Type = TransactionType.TopUp,
                CreatedAt = DateTime.UtcNow,
                Status = TransactionStatus.Pending
            };
            await _transactionRepository.AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new WalletResponseDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt
            };
        }
    }
}
