using MediatR;
using Neighborhood.Services.Application.Offers.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Offers;

namespace Neighborhood.Services.Application.Offers.Queries.GetTechnicianOffers
{
    // Returns the authenticated technician's own offers (paged + optional status filter).
    public class GetTechnicianOffersQuery : IRequest<PagedResult<OfferDto>>
    {
        public OfferStatus? Status { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
