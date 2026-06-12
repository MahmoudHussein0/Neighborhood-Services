using MediatR;
using Neighborhood.Services.Domain.Disputes;

namespace Neighborhood.Services.Application.Bookings.Commands.RaiseDisputeCommands
{
    // Raises a dispute on a booking: validates the booking is disputable, flips it to Disputed,
    // then creates the dispute record (delegated to the Disputes module's CreateDisputeCommand).
    public class RaiseDisputeCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        public DisputeType DisputeType { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
