using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Offers.DTOs;
using Neighborhood.Services.Application.Offers.Interfaces;
using Neighborhood.Services.Domain.Offers;

namespace Neighborhood.Services.Application.Offers.Queries.GetOfferById
{
    public class GetOfferByIdQueryHandler : IRequestHandler<GetOfferByIdQuery, OfferDto>
    {
        private readonly IOfferRepository _offerRepository;

        public GetOfferByIdQueryHandler(IOfferRepository offerRepository)
        {
            _offerRepository = offerRepository;
        }

        public async Task<OfferDto> Handle(GetOfferByIdQuery request, CancellationToken cancellationToken)
        {
            var offer = await _offerRepository.GetOfferWithDetailsAsync(request.OfferId);
            if (offer is null)
                throw new NotFoundException(nameof(Offer), request.OfferId);

            return new OfferDto
            {
                Id = offer.Id,
                ServiceRequestId = offer.ServiceRequestId,
                TechnicianId = offer.TechnicianId,
                Price = offer.Price,
                EstimatedDuration = offer.EstimatedDuration,
                Message = offer.Message,
                ScheduledAt = offer.ScheduledAt,
                Status = offer.Status,
                CreatedAt = offer.CreatedAt
            };
        }
    }
}
