using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Neighborhood.Services.Application.Escrows.DTOs;
using Neighborhood.Services.Application.Escrows.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Transactions.Commands.CreateTransaction;
using Neighborhood.Services.Application.Transactions.Interfaces;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Application.Invoices.Interfaces;
using Neighborhood.Services.Domain.Escrows;
using Neighborhood.Services.Domain.Transactions;
using Neighborhood.Services.Domain.Invoices;
namespace Neighborhood.Services.Application.Escrows.Commands.ReleaseEscrow
{
    public class ReleaseEscrowCommandHandler : IRequestHandler<ReleaseEscrowCommand, EscrowResponseDto>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ReleaseEscrowCommandHandler(
            IEscrowRepository escrowRepository, 
            ITransactionRepository transactionRepository, 
            IWalletRepository walletRepository, 
            IInvoiceRepository invoiceRepository,
            IUnitOfWork unitOfWork)
        {
            _escrowRepository = escrowRepository;
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<EscrowResponseDto> Handle(ReleaseEscrowCommand request, CancellationToken cancellationToken)
        {
            var escrow = (await _escrowRepository.GetByConditionAsync(
                e => e.Id == request.EscrowId, includeProperties: "Booking.Technician")).FirstOrDefault()
                ?? throw new KeyNotFoundException($"Escrow with ID {request.EscrowId} not found.");

            if (escrow.Status != EscrowStatus.Held)
                throw new InvalidOperationException($"Escrow is not in Held status");

            if (escrow.Amount <= 0)
                throw new InvalidOperationException("Escrow amount must be greater than zero.");

            var releasedAt = DateTime.UtcNow;

            var technicianWallet = await _walletRepository.GetByUserIdAsync(
                escrow.Booking.Technician.ApplicationUserId) ??
                throw new NotFoundException($"Technician's wallet not found.");

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _walletRepository.CreditAsync(technicianWallet.Id, escrow.Amount);
                await _escrowRepository.ReleaseAsync(request.EscrowId);
                
                var transaction = new Transaction
                {
                    FromWalletId = escrow.WalletId,
                    ToWalletId = technicianWallet.Id,
                    PaymentMethodId = null,
                    Amount = escrow.Amount,
                    Fee = 0,
                    Currency = "EGP",
                    Type = TransactionType.Transfer,
                    Status = TransactionStatus.Completed,
                    CreatedAt = releasedAt
                };
                
                await _transactionRepository.AddAsync(transaction);

                var invoice = new Invoice
                {
                    BookingId = escrow.BookingId,
                    CustomerId = escrow.Booking.CustomerId,
                    TechnicianId = escrow.Booking.TechnicianId,
                    Amount = escrow.Amount,
                    Tax = 0,
                    TotalAmount = escrow.Amount,
                    Transaction = transaction,
                    Status = InvoiceStatus.Paid,
                    IssuedAt = releasedAt,
                    PaidAt = releasedAt
                };
                
                await _invoiceRepository.AddAsync(invoice);

            }, cancellationToken);

            return new EscrowResponseDto
            {
                Id = escrow.Id,
                BookingId = escrow.BookingId,
                WalletId = escrow.WalletId,
                Amount = escrow.Amount,
                Status = EscrowStatus.Released,
                HeldAt = escrow.HeldAt,
                ReleasedAt = releasedAt
            };
        }
    }
}
