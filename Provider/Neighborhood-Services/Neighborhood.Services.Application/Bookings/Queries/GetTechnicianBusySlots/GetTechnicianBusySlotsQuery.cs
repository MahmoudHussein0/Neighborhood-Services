using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;

namespace Neighborhood.Services.Application.Bookings.Queries.GetTechnicianBusySlots
{
    // The technician's busy windows in [From, To) — used by the booking UI to grey out
    // times the tech is already booked.
    public class GetTechnicianBusySlotsQuery : IRequest<IEnumerable<BusySlotDto>>
    {
        public int TechnicianId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
