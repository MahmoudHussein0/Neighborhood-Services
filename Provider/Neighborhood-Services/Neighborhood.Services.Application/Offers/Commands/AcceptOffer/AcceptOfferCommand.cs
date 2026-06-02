using MediatR;

namespace Neighborhood.Services.Application.Offers.Commands.AcceptOffer
{
    // Accepting an offer creates the (Confirmed) bidding booking and returns its id.
    public class AcceptOfferCommand : IRequest<int>
    {
        public int OfferId { get; set; }
    }
}
