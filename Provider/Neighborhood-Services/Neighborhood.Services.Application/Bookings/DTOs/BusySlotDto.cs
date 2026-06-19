namespace Neighborhood.Services.Application.Bookings.DTOs
{
    // One window during which a technician is occupied by a confirmed booking.
    public class BusySlotDto
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
