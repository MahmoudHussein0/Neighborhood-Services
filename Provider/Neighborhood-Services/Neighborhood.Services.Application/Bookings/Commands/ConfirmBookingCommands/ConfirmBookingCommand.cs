using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.ConfirmBookingCommands
{
    public class ConfirmBookingCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        // The confirming customer is resolved from the authenticated user in the handler.
    }
}
