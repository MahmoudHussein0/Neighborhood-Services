using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.CompleteBookingCommands
{
    public class CompleteBookingCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        // TODO: CompletedBy (technician) will come from current user service when available
    }
}
