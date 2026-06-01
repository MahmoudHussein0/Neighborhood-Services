using MediatR;

namespace Neighborhood.Services.Application.Offers.Commands.RejectOffer
{
    public class RejectOfferCommand : IRequest<bool>
    {
        public int OfferId { get; set; }
    }
}
