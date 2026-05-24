using Neighborhood.Services.Domain.AiAnalyses;
using Neighborhood.Services.Domain.BookingImages;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Domain.RecurringBookings;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Domain.Bookings
{
    public class Booking
    {
        //----- Self Prop
        public int Id { get; set; }
        public BookingType BookingType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public decimal EstimatedPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public bool ClientConfirmed { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // ---------------Foreign Keys 
        public int CustomerId { get; set; }
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
        public int? OfferId { get; set; }
        public int? ServiceRequestId { get; set; }
        public int? PromoCodeId { get; set; }
        public int? RecurringBookingId { get; set; }
        public int? CancelledBy { get; set; }

        // Navigation Properties
        //public Customer Customer { get; set; }
        //public Technician Technician { get; set; }
        //public ProblemType ProblemType { get; set; }
        public Offer? Offer { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }
        //public PromoCode? PromoCode { get; set; }
        public RecurringBooking? RecurringBooking { get; set; }
        //public User? CancelledByUser { get; set; }

        public ICollection<BookingImage> BookingImages { get; set; } = new HashSet<BookingImage>();
        //public ICollection<PromoCodeUsage> PromoCodeUsages { get; set; }
        public AiAnalysis? AiAnalysis { get; set; }
        //public Escrow? Escrow { get; set; }
        //public Invoice? Invoice { get; set; }
        //public Dispute? Dispute { get; set; }
        //public Review? Review { get; set; }
        //public Conversation? Conversation { get; set; }
        //public SupportTicket? SupportTicket { get; set; }
    }
}
