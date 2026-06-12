using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.StaffCancelBookingCommands
{
    // Admin/staff cancellation — deliberately SEPARATE from the customer/technician cancel.
    // Just cancels: no refund, no reassign (out of scope by design). Authorized at the endpoint
    // (Roles = "Staff"), so it doesn't need the party-ownership checks the customer/tech cancel has.
    public class StaffCancelBookingCommand : IRequest<bool>
    {
        public int BookingId { get; set; }
        public string CancellationReason { get; set; } = string.Empty;
    }
}
