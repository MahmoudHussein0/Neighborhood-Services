using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;
using Neighborhood.Services.Application.Invoices.Interfaces;
namespace Neighborhood.Services.Application.Invoices.Queries.GetInvoiceByCustomerId
{
    public class GetInvoicesByCustomerIdQueryHandler : IRequestHandler<GetInvoicesByCustomerIdQuery, IEnumerable<InvoiceResponseDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoicesByCustomerIdQueryHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<IEnumerable<InvoiceResponseDto>> Handle(GetInvoicesByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetByCustomerIdAsync(request.CustomerId);

            return invoices.Select(i => new InvoiceResponseDto
            {
                Id = i.Id,
                BookingId = i.BookingId,
                TransactionId = i.TransactionId,
                CustomerId = i.CustomerId,
                TechnicianId = i.TechnicianId,
                Amount = i.Amount,
                Tax = i.Tax,
                TotalAmount = i.TotalAmount,
                Status = i.Status,
                IssuedAt = i.IssuedAt,
                PaidAt = i.PaidAt,
                VoidedAt = i.VoidedAt
            });
        }
    }
}