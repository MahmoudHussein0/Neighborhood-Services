using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;

namespace Neighborhood.Services.Application.Invoices.Queries.GetMyInvoices
{
    public class GetMyInvoicesQuery : IRequest<IEnumerable<InvoiceResponseDto>>
    {
    }
}
