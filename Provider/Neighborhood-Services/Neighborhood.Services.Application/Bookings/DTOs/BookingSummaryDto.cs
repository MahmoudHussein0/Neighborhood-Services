using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.DTOs
{
    public class BookingSummaryDto
    {
        public int Id { get; set; }
        public BookingType BookingType { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public decimal EstimatedPrice { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
