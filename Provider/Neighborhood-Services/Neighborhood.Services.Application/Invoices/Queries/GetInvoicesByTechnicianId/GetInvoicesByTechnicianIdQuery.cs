using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;
namespace Neighborhood.Services.Application.Invoices.Queries.GetInvoicesByTechnicianId
{
    public class GetInvoicesByTechnicianIdQuery : IRequest<IEnumerable<InvoiceResponseDto>>
    {
        public int TechnicianId { get; set; }
    }
}