using Neighborhood.Services.Domain.Shared;
namespace Neighborhood.Services.Domain.Invoices
{
    public class Invoice : BaseEntity
    {
        public int BookingId { get; set; }
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public int TechnicianId { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
        public DateTime? PaidAt { get; set; }
        public DateTime? VoidedAt { get; set; }
    }
}