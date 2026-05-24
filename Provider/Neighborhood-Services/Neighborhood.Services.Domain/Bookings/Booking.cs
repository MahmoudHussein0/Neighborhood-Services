using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Domain.Booking
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
    }
}
