using Neighborhood.Services.Domain.RecurringBookings;

namespace Neighborhood.Services.Application.RecurringBookings.DTOs
{
    public class RecurringBookingDto
    {
        public int Id { get; set; }
        // Customer-supplied job description + optional reference photo (shown to the technician).
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string Address { get; set; } = string.Empty;
        public RecurringPattern Pattern { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public TimeOnly TimeOfDay { get; set; }
        public int DurationMinutes { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public RecurringBookingStatus Status { get; set; }
        public decimal? AgreedPrice { get; set; }
        public int CustomerId { get; set; }
        public int TechnicianId { get; set; }
        public int ProblemTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
