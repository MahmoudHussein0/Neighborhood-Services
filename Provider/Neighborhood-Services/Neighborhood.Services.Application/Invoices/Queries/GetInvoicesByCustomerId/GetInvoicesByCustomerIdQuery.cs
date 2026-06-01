using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;
namespace Neighborhood.Services.Application.Invoices.Queries.GetInvoiceByCustomerId
{
    public class GetInvoicesByCustomerIdQuery : IRequest<IEnumerable<InvoiceResponseDto>>
    {
        public int CustomerId { get; set; }
    }
}