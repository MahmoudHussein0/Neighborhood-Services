using MediatR;
using Neighborhood.Services.Application.Escrows.DTOs;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Escrows.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Escrows;
using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Application.Escrows.Commands.CreateEscrow
{
    public class CreateEscrowCommandHandler : IRequestHandler<CreateEscrowCommand, EscrowResponseDto>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateEscrowCommandHandler(
            IEscrowRepository escrowRepository,
            ITransactionRepository transactionRepository,
            IWalletRepository walletRepository,
            IUnitOfWork unitOfWork)
        {
            _escrowRepository = escrowRepository;
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<EscrowResponseDto> Handle(CreateEscrowCommand request, CancellationToken cancellationToken)
        {
            if (request.Amount <= 0)
                throw new InvalidOperationException("Escrow amount must be greater than zero.");

            var existing = await _escrowRepository.GetByBookingIdAsync(request.BookingId);

            if (existing is not null)
                throw new InvalidOperationException($"Escrow for booking {request.BookingId} already exists.");

            var wallet = await _walletRepository.GetByIdAsync(request.WalletId)
                ?? throw new KeyNotFoundException($"Wallet with ID {request.WalletId} not found.");

            if (wallet.Balance < request.Amount)
                throw new BadRequestException("Insufficient wallet balance to confirm this booking.");

            var escrow = new Escrow
            {
                BookingId = request.BookingId,
                WalletId = request.WalletId,
                Amount = request.Amount,
                Status = EscrowStatus.Held,
                HeldAt = DateTime.UtcNow
            };

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _walletRepository.DebitAsync(wallet.Id, request.Amount);
                await _escrowRepository.AddAsync(escrow);
                await _transactionRepository.AddAsync(new Transaction
                {
                    FromWalletId = wallet.Id,
                    ToWalletId = null,
                    PaymentMethodId = null,
                    Amount = request.Amount,
                    Fee = 0,
                    Currency = "EGP",
                    Type = TransactionType.BookingPayment,
                    Status = TransactionStatus.Completed,
                    CreatedAt = DateTime.UtcNow
                });
            }, cancellationToken);

            return new EscrowResponseDto
            {
                Id = escrow.Id,
                BookingId = escrow.BookingId,
                WalletId = escrow.WalletId,
                Amount = escrow.Amount,
                Status = escrow.Status,
                HeldAt = escrow.HeldAt,
                ReleasedAt = escrow.ReleasedAt
            };
        }
    }
}
