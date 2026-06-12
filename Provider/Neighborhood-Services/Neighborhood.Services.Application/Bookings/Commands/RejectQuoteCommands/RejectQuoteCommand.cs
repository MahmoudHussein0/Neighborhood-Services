using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.RejectQuoteCommands
{
    // Customer rejects the technician's quote. The booking goes back to Pending so
    // the technician can re-quote (or the customer can cancel from there).
    public class RejectQuoteCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
    }
}
