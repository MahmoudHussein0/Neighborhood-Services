using MediatR;
using Neighborhood.Services.Application.Offers.DTOs;

namespace Neighborhood.Services.Application.Offers.Queries.GetTechnicianOffers
{
    // Returns the authenticated technician's own offers.
    public class GetTechnicianOffersQuery : IRequest<IEnumerable<OfferDto>>
    {
    }
}
