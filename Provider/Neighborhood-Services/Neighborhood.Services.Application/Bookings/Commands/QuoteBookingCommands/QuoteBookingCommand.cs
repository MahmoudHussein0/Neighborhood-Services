using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.QuoteBookingCommands
{
    // Sent by the technician on a Pending booking. Sets FinalPrice + DurationMinutes
    // and moves the booking to Quoted (= customer's turn to accept / reject).
    public class QuoteBookingCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        public decimal FinalPrice { get; set; }
        public int DurationMinutes { get; set; }
    }
}
