using Neighborhood.Services.Application.Invoices.DTOs;

namespace Neighborhood.Services.Application.Invoices.Services
{
    public interface IInvoicePdfService
    {
        byte[] GenerateInvoicePdf(InvoiceResponseDto invoice);
    }
}
