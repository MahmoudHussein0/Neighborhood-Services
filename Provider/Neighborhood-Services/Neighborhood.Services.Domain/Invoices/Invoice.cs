using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Customers;
using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Technicians;
using Neighborhood.Services.Domain.Transactions;
namespace Neighborhood.Services.Domain.Invoices
{
    public class Invoice : BaseEntity<int>
    {
        public int BookingId { get; set; }
        public int? TransactionId { get; set; }
        public int CustomerId { get; set; }
        public int TechnicianId { get; set; }
        public decimal Amount { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        /// <summary>Original price before any promo code was applied.</summary>
        public decimal BaseAmount { get; set; }
        /// <summary>Amount saved via promo code (0 if no promo code was used).</summary>
        public decimal DiscountAmount { get; set; }
        /// <summary>The promo code string that was applied, e.g. "WELCOME10". Null if none.</summary>
        public string? PromoCodeApplied { get; set; }
        public Booking Booking { get; set; } = null!;
        public Transaction? Transaction { get; set; }
        public Customer Customer { get; set; } = null!;
        public Technician Technician { get; set; } = null!;
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
        public DateTime? PaidAt { get; set; }
        public DateTime? VoidedAt { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    }
}