using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Offers.Interfaces;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.Offers.Commands.RejectOffer
{
    public class RejectOfferCommandHandler : IRequestHandler<RejectOfferCommand, bool>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public RejectOfferCommandHandler(
            IOfferRepository offerRepository,
            IServiceRequestRepository serviceRequestRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _offerRepository = offerRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RejectOfferCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var offer = await _offerRepository.GetOfferWithDetailsAsync(request.OfferId);
            if (offer is null)
                throw new NotFoundException(nameof(Offer), request.OfferId);

            // Only the customer who owns the service request can reject its offers
            var serviceRequest = await _serviceRequestRepository.GetServiceRequestWithDetailsAsync(offer.ServiceRequestId);
            if (serviceRequest is null)
                throw new NotFoundException(nameof(ServiceRequest), offer.ServiceRequestId);

            if (serviceRequest.Customer.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this service request.");

            if (offer.Status != OfferStatus.Pending)
                throw new BadRequestException($"Only a pending offer can be rejected. Current status: {offer.Status}.");

            offer.Status = OfferStatus.Rejected;
            await _offerRepository.UpdateAsync(offer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
