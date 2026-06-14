using MediatR;

namespace Neighborhood.Services.Application.Bookings.Commands.AcceptQuoteCommands
{
    // Customer accepts the technician's quote — escrow is held @ FinalPrice and
    // the booking transitions Quoted -> Confirmed.
    public class AcceptQuoteCommand : IRequest<bool>
    {
        public int BookingId { get; set; }

        // Optional promo code applied at accept-time — discounts the quoted FinalPrice
        // before escrow is held. Null/blank means no discount.
        public string? PromoCode { get; set; }
    }
}
