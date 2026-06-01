using MediatR;
using Neighborhood.Services.Application.Offers.DTOs;

namespace Neighborhood.Services.Application.Offers.Queries.GetOffersByServiceRequest
{
    public class GetOffersByServiceRequestQuery : IRequest<IEnumerable<OfferDto>>
    {
        public int ServiceRequestId { get; set; }
    }
}
