using System.Text;
using Neighborhood.Services.Application.Invoices.DTOs;
using Neighborhood.Services.Application.Invoices.Services;

namespace Neighborhood.Services.Infrastructure.Services.Invoices
{
    public class InvoicePdfService : IInvoicePdfService
    {
        public byte[] GenerateInvoicePdf(InvoiceResponseDto invoice)
        {
            var content = new StringBuilder()
                .AppendLine("Invoice")
                .AppendLine($"Invoice ID: {invoice.Id}")
                .AppendLine($"Booking ID: {invoice.BookingId}")
                .AppendLine($"Customer ID: {invoice.CustomerId}")
                .AppendLine($"Technician ID: {invoice.TechnicianId}")
                .AppendLine($"Amount: {invoice.Amount}")
                .AppendLine($"Tax: {invoice.Tax}")
                .AppendLine($"Total: {invoice.TotalAmount}")
                .AppendLine($"Status: {invoice.Status}")
                .AppendLine($"Issued At: {invoice.IssuedAt:O}")
                .AppendLine($"Paid At: {invoice.PaidAt:O}")
                .AppendLine($"Voided At: {invoice.VoidedAt:O}")
                .ToString();

            return Encoding.UTF8.GetBytes(content);
        }
    }
}
