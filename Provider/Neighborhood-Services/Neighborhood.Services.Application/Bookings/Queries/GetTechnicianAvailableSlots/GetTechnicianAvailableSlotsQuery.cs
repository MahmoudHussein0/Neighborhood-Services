using MediatR;

namespace Neighborhood.Services.Application.Bookings.Queries.GetTechnicianAvailableSlots
{
    // Free start-times a customer can book for a technician on a given date:
    // working hours for that weekday, minus the tech's confirmed-booking windows, minus past
    // times — already cut into slots so the booking UI can render them as clickable chips.
    public class GetTechnicianAvailableSlotsQuery : IRequest<IEnumerable<DateTime>>
    {
        public int TechnicianId { get; set; }
        public DateTime Date { get; set; }
        public int SlotMinutes { get; set; } = 30;
    }
}
