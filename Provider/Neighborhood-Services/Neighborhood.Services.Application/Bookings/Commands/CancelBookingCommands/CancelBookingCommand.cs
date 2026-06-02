using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.CancelBookingCommands
{
    public class CancelBookingCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        // CancelledBy is resolved from the authenticated user in the handler.
        public string CancellationReason { get; set; } = string.Empty;
    }
}
