using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.DTOs
{
    // Richer summary used only by GET /api/bookings/mine — needed by the customer
    // and technician pages, which surface the quoted FinalPrice + DurationMinutes
    // and want the (TechnicianId, ProblemTypeId) so the UI can look up the tech's
    // pricing range for that problem type.
    public class MyBookingSummaryDto
    {
        public int Id { get; set; }
        public BookingType BookingType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public decimal EstimatedPrice { get; set; }
        // Set by the technician on the Direct flow (Pending -> Quoted). Stays 0 until then.
        public decimal FinalPrice { get; set; }
        public int? DurationMinutes { get; set; }
        public BookingStatus Status { get; set; }
        // True once the customer has confirmed a Completed booking (escrow released).
        // Used to hide the "Confirm completed" action after it's done.
        public bool ClientConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        // Both parties' ids + display names. Each side of /bookings/mine shows the OTHER party:
        // the technician's Jobs page shows the customer, the customer's Bookings page the technician.
        public int TechnicianId { get; set; }
        public string TechnicianName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int ProblemTypeId { get; set; }
        // From Location (Point): Y = latitude, X = longitude. Lets the UI open the spot in Maps.
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // True if the current user (customer or technician) has already left a review for this booking.
        // Drives the "Leave a review" vs "Reviewed ✓" state on the bookings/jobs pages.
        public bool HasReview { get; set; }
    }
}
