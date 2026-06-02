using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.AcceptBookingCommands
{
    public class AcceptBookingCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        // The technician estimates how long the job will take when accepting.
        public int DurationMinutes { get; set; }
        // TODO: AcceptedBy (technician) will come from current user service when available
    }
}
