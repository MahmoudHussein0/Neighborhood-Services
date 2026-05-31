using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.CancelBookingCommands
{
    public class CancelBookingCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        public string CancelledBy { get; set; } = string.Empty;
        public string CancellationReason { get; set; } = string.Empty;
    }
}
