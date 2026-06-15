using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
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
        private readonly INotificationService _notificationService;
        private readonly ILogger<RejectOfferCommandHandler> _logger;

        public RejectOfferCommandHandler(
            IOfferRepository offerRepository,
            IServiceRequestRepository serviceRequestRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ILogger<RejectOfferCommandHandler> logger)
        {
            _offerRepository = offerRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
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

            // Notify the technician their offer was rejected — best effort.
            try
            {
                if (!string.IsNullOrEmpty(offer.Technician?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        offer.Technician.ApplicationUserId,
                        $"Your offer on service request #{serviceRequest.Id} was rejected.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Reject-offer notification failed for offer {Id}.", offer.Id);
            }

            return true;
        }
    }
}
