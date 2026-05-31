using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.ConfirmBookingCommands
{
    public class ConfirmBookingCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        // TODO: ConfirmedBy will come from current user service when available
        public string ConfirmedBy { get; set; } = string.Empty;
    }
}
