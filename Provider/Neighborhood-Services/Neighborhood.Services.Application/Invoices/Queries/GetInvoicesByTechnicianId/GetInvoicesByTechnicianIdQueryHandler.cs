using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;
using Neighborhood.Services.Application.Invoices.Interfaces;
namespace Neighborhood.Services.Application.Invoices.Queries.GetInvoicesByTechnicianId
{
    public class GetInvoicesByTechnicianIdQueryHandler : IRequestHandler<GetInvoicesByTechnicianIdQuery, IEnumerable<InvoiceResponseDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public GetInvoicesByTechnicianIdQueryHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<IEnumerable<InvoiceResponseDto>> Handle(GetInvoicesByTechnicianIdQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetByTechnicianIdAsync(request.TechnicianId);

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