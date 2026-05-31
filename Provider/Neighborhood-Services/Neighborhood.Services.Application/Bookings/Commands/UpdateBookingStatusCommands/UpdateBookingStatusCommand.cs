using MediatR;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.UpdateBookingStatusCommands
{
    public class UpdateBookingStatusCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        public BookingStatus NewStatus { get; set; }
    }
}
