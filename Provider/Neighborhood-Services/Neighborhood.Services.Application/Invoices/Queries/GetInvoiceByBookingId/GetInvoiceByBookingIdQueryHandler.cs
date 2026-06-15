using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;
using Neighborhood.Services.Application.Invoices.Interfaces;
namespace Neighborhood.Services.Application.Invoices.Queries.GetInvoiceByBookingId
{
    public class GetInvoiceByBookingIdQueryHandler : IRequestHandler<GetInvoiceByBookingIdQuery, InvoiceResponseDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoiceByBookingIdQueryHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<InvoiceResponseDto> Handle(GetInvoiceByBookingIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByBookingIdAsync(request.BookingId)
                ?? throw new KeyNotFoundException($"Invoice for booking ID {request.BookingId} not found.");

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