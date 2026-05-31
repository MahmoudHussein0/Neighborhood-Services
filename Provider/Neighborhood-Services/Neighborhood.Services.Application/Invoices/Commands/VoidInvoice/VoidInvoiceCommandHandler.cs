using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;
using Neighborhood.Services.Application.Invoices.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Invoices;
namespace Neighborhood.Services.Application.Invoices.Commands.VoidInvoice
{
    public class VoidInvoiceCommandHandler : IRequestHandler<VoidInvoiceCommand, InvoiceResponseDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VoidInvoiceCommandHandler(IInvoiceRepository invoiceRepository, IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<InvoiceResponseDto> Handle(VoidInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId)
                ?? throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");

            if (invoice.Status == InvoiceStatus.Voided)
                throw new InvalidOperationException($"Invoice with ID {request.InvoiceId} is already voided.");

            if (invoice.Status is InvoiceStatus.Paid or InvoiceStatus.Refunded)
                throw new InvalidOperationException("Only unpaid invoices can be voided.");

            await _invoiceRepository.VoidAsync(request.InvoiceId);
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
                Status = InvoiceStatus.Voided,
                IssuedAt = invoice.IssuedAt,
                PaidAt = invoice.PaidAt,
                VoidedAt = DateTime.UtcNow
            };
        }
    }
}
