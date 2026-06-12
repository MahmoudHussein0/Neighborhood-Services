using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.DTOs
{
    // Row in the staff Bookings oversight list — includes WHO (customer + technician names).
    public class StaffBookingDto
    {
        public int Id { get; set; }
        public BookingType BookingType { get; set; }
        public BookingStatus Status { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string TechnicianName { get; set; } = string.Empty;
        public decimal EstimatedPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public DateTime ScheduledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Address { get; set; } = string.Empty;
    }
}
