using Neighborhood.Services.Domain.Invoices;
namespace Neighborhood.Services.Application.Invoices.DTOs
{
    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int? TransactionId { get; set; }
        public int CustomerId { get; set; }
        public int TechnicianId { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? VoidedAt {  get; set; }
    }
}