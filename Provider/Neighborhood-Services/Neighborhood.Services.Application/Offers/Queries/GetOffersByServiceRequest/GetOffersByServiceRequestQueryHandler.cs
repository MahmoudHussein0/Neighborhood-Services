using MediatR;
using Neighborhood.Services.Application.Offers.DTOs;
using Neighborhood.Services.Application.Offers.Interfaces;

namespace Neighborhood.Services.Application.Offers.Queries.GetOffersByServiceRequest
{
    public class GetOffersByServiceRequestQueryHandler : IRequestHandler<GetOffersByServiceRequestQuery, IEnumerable<OfferDto>>
    {
        private readonly IOfferRepository _offerRepository;

        public GetOffersByServiceRequestQueryHandler(IOfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public async Task<IEnumerable<OfferDto>> Handle(GetOffersByServiceRequestQuery request, CancellationToken cancellationToken)
        {
            var offers = await _offerRepository.GetOffersByServiceRequestAsync(request.ServiceRequestId);

            return offers.Select(MapToDto);
        }

        private static OfferDto MapToDto(Domain.Offers.Offer o) => new()
        {
            Id = o.Id,
            ServiceRequestId = o.ServiceRequestId,
            TechnicianId = o.TechnicianId,
            Price = o.Price,
            EstimatedDuration = o.EstimatedDuration,
            Message = o.Message,
            ScheduledAt = o.ScheduledAt,
            Status = o.Status,
            CreatedAt = o.CreatedAt
        };
    }
}
