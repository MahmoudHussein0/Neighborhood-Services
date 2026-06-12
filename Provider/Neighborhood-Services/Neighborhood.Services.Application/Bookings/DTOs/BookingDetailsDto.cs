using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.DTOs
{
    public class BookingDetailsDto
    {
        public int Id { get; set; }
        public BookingType BookingType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public decimal EstimatedPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public bool ClientConfirmed { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CustomerId { get; set; }
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
        public int? OfferId { get; set; }
        public int? ServiceRequestId { get; set; }
        // From Location (Point): Y = latitude, X = longitude. Lets the UI open the spot in Maps.
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
