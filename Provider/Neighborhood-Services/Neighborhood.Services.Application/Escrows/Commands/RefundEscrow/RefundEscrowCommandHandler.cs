using MediatR;
using Neighborhood.Services.Application.Escrows.DTOs;
using Neighborhood.Services.Application.Escrows.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Escrows;
using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Application.Escrows.Commands.RefundEscrow
{
    public class RefundEscrowCommandHandler : IRequestHandler<RefundEscrowCommand, EscrowResponseDto>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RefundEscrowCommandHandler(
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

        public async Task<EscrowResponseDto> Handle(RefundEscrowCommand request, CancellationToken cancellationToken)
        {
            var escrow = (await _escrowRepository.GetByConditionAsync(
                e => e.Id == request.EscrowId,
                includeProperties: "Booking.Customer"))
                .FirstOrDefault()
                ?? throw new KeyNotFoundException($"Escrow with ID {request.EscrowId} not found.");

            if (escrow.Status != EscrowStatus.Held)
                throw new InvalidOperationException("Escrow is not in Held status");

            if (escrow.Amount <= 0)
                throw new InvalidOperationException("Escrow amount must be greater than zero.");

            var customerWallet = await _walletRepository.GetByUserIdAsync(
                escrow.Booking.Customer.ApplicationUserId)
                ?? throw new KeyNotFoundException("Customer wallet not found");

            var refundedAt = DateTime.UtcNow;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _walletRepository.CreditAsync(customerWallet.Id, escrow.Amount);
                await _escrowRepository.RefundAsync(request.EscrowId);

                await _transactionRepository.AddAsync(new Transaction
                {
                    FromWalletId = null,             // Money comes from the escrow hold, not a wallet
                    ToWalletId = customerWallet.Id,
                    PaymentMethodId = null,
                    Amount = escrow.Amount,
                    Fee = 0,
                    Currency = "EGP",
                    Type = TransactionType.Refund,
                    Status = TransactionStatus.Completed,
                    CreatedAt = refundedAt
                });
            }, cancellationToken);

            return new EscrowResponseDto
            {
                Id = escrow.Id,
                BookingId = escrow.BookingId,
                WalletId = escrow.WalletId,
                Amount = escrow.Amount,
                Status = EscrowStatus.Refunded,
                HeldAt = escrow.HeldAt,
                ReleasedAt = refundedAt
            };
        }
    }
}
