using MediatR;
using Neighborhood.Services.Application.Invoices.DTOs;

namespace Neighborhood.Services.Application.Invoices.Commands.CreateInvoice
{
    public class CreateInvoiceCommand : IRequest<InvoiceResponseDto>
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public int TechnicianId { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal BaseAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public string? PromoCodeApplied { get; set; }
        public int? TransactionId { get; set; }
    }
}
