using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;
using Neighborhood.Services.Application.Invoices.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Invoices;

namespace Neighborhood.Services.Application.Invoices.Commands.CreateInvoice
{
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceResponseDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceResponseDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var existingInvoice = await _invoiceRepository.GetByBookingIdAsync(request.BookingId);
            if (existingInvoice is not null)
                throw new InvalidOperationException($"Invoice for booking {request.BookingId} already exists.");

            var invoice = new Invoice
            {
                BookingId = request.BookingId,
                CustomerId = request.CustomerId,
                TechnicianId = request.TechnicianId,
                Amount = request.Amount,
                Tax = request.Tax,
                TotalAmount = request.TotalAmount,
                BaseAmount = request.BaseAmount,
                DiscountAmount = request.DiscountAmount,
                PromoCodeApplied = request.PromoCodeApplied,
                TransactionId = request.TransactionId,
                Status = request.TransactionId.HasValue ? InvoiceStatus.Paid : InvoiceStatus.Unpaid,
                IssuedAt = DateTime.UtcNow,
                PaidAt = request.TransactionId.HasValue ? DateTime.UtcNow : null
            };

            await _invoiceRepository.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new InvoiceResponseDto
            {
                Id = invoice.Id,
                BookingId = invoice.BookingId,
                TransactionId = invoice.TransactionId,
                CustomerId = invoice.CustomerId,
                TechnicianId = invoice.TechnicianId,
                Amount = invoice.Amount,
                Tax = invoice.Tax,
                TotalAmount = invoice.TotalAmount,
                BaseAmount = invoice.BaseAmount,
                DiscountAmount = invoice.DiscountAmount,
                PromoCodeApplied = invoice.PromoCodeApplied,
                Status = invoice.Status,
                IssuedAt = invoice.IssuedAt,
                PaidAt = invoice.PaidAt,
                VoidedAt = invoice.VoidedAt
            };
        }
    }
}
