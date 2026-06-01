using MediatR;
using Neighborhood.Services.Application.Offers.DTOs;

namespace Neighborhood.Services.Application.Offers.Queries.GetOfferById
{
    public class GetOfferByIdQuery : IRequest<OfferDto>
    {
        public int OfferId { get; set; }
    }
}
