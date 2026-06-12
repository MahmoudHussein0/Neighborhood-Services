using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.AcceptQuoteCommands
{
    // Customer accepts the technician's quote — escrow is held @ FinalPrice and
    // the booking transitions Quoted -> Confirmed.
    public class AcceptQuoteCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
    }
}
