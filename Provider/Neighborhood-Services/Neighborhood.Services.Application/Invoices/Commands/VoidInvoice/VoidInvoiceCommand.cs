using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;
namespace Neighborhood.Services.Application.Invoices.Commands.VoidInvoice
{
    public class VoidInvoiceCommand : IRequest<InvoiceResponseDto>
    {
        public int InvoiceId { get; set; }
    }
}