using MediatR;

namespace Neighborhood.Services.Application.Offers.Commands.AcceptOffer
{
    // Accepting an offer creates the (Confirmed) bidding booking and returns its id.
    public class AcceptOfferCommand : IRequest<int>
    {
        public int OfferId { get; set; }

        // Optional promo code applied at accept-time — discounts the offer price
        // (now the booking's FinalPrice) before escrow is held. Null/blank means no discount.
        public string? PromoCode { get; set; }
    }
}
