using MediatR;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Conversations.Commands;
using Neighborhood.Services.Application.Escrows.Commands.CreateEscrow;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Offers.Interfaces;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.Offers.Commands.AcceptOffer
{
    public class AcceptOfferCommandHandler : IRequestHandler<AcceptOfferCommand, int>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public AcceptOfferCommandHandler(
            IOfferRepository offerRepository,
            IServiceRequestRepository serviceRequestRepository,
            IBookingRepository bookingRepository,
            IWalletRepository walletRepository,
            ICurrentUserService currentUserService,
            IMediator mediator,
            IUnitOfWork unitOfWork)
        {
            _offerRepository = offerRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _bookingRepository = bookingRepository;
            _walletRepository = walletRepository;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(AcceptOfferCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var offer = await _offerRepository.GetOfferWithDetailsAsync(request.OfferId);
            if (offer is null)
                throw new NotFoundException(nameof(Offer), request.OfferId);

            if (offer.Status != OfferStatus.Pending)
                throw new BadRequestException($"Only a pending offer can be accepted. Current status: {offer.Status}.");

            // Load the service request with its customer (for ownership + wallet)
            var serviceRequest = await _serviceRequestRepository.GetServiceRequestWithDetailsAsync(offer.ServiceRequestId);
            if (serviceRequest is null)
                throw new NotFoundException(nameof(ServiceRequest), offer.ServiceRequestId);

            // Only the customer who owns the request can accept an offer on it
            if (serviceRequest.Customer.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this service request.");

            if (serviceRequest.Status != ServiceRequestStatus.Open)
                throw new BadRequestException("This service request is no longer open.");

            var start = offer.ScheduledAt;
            var end = start.AddMinutes(offer.EstimatedDuration);

            // The technician must still be free at the proposed time
            var hasOverlap = await _bookingRepository.HasOverlappingConfirmedBookingAsync(offer.TechnicianId, start, end);
            if (hasOverlap)
                throw new ConflictException("The technician is no longer available at the proposed time.");

            // Fail fast on insufficient funds before creating the booking, so the
            // escrow step (run after the booking is saved) is very unlikely to fail.
            var customerWallet = await _walletRepository.GetByUserIdAsync(serviceRequest.Customer.ApplicationUserId)
                ?? throw new NotFoundException("Wallet", serviceRequest.Customer.ApplicationUserId);
            if (customerWallet.Balance < offer.Price)
                throw new BadRequestException("Insufficient wallet balance to confirm this booking.");

            var booking = new Booking
            {
                CustomerId = serviceRequest.CustomerId,
                TechnicianId = offer.TechnicianId,
                ProblemTypeId = serviceRequest.ProblemTypeId,
                OfferId = offer.Id,
                ServiceRequestId = serviceRequest.Id,
                BookingType = BookingType.Bidding,
                Description = serviceRequest.Description,
                Address = serviceRequest.Address,
                ScheduledAt = offer.ScheduledAt,
                DurationMinutes = offer.EstimatedDuration,
                EstimatedPrice = offer.Price,
                FinalPrice = 0,
                Status = BookingStatus.Confirmed,
                Location = serviceRequest.Location,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _bookingRepository.AddAsync(booking);

            // Accept this offer, close the request
            offer.Status = OfferStatus.Accepted;
            await _offerRepository.UpdateAsync(offer);

            serviceRequest.Status = ServiceRequestStatus.Closed;
            await _serviceRequestRepository.UpdateAsync(serviceRequest);

            // Reject all other offers on this service request
            var otherOffers = await _offerRepository.GetOffersByServiceRequestAsync(serviceRequest.Id);
            foreach (var other in otherOffers)
            {
                if (other.Id != offer.Id && other.Status == OfferStatus.Pending)
                {
                    other.Status = OfferStatus.Rejected;
                    await _offerRepository.UpdateAsync(other);
                }
            }

            // Reject this technician's other pending offers that overlap the now-booked time
            var technicianOffers = await _offerRepository.GetTechnicianOffersAsync(offer.TechnicianId);
            foreach (var techOffer in technicianOffers)
            {
                if (techOffer.Id == offer.Id || techOffer.Status != OfferStatus.Pending)
                    continue;

                var otherStart = techOffer.ScheduledAt;
                var otherEnd = otherStart.AddMinutes(techOffer.EstimatedDuration);
                if (otherStart < end && start < otherEnd) // intervals overlap
                {
                    techOffer.Status = OfferStatus.Rejected;
                    await _offerRepository.UpdateAsync(techOffer);
                }
            }

            try
            {
                // One SaveChanges commits the booking + all status changes atomically.
                // The DB unique index on (TechnicianId, ScheduledAt) backstops the overlap race.
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                throw new ConflictException("The technician is no longer available at the proposed time.");
            }

            // Hold the customer's payment in escrow (manages its own transaction).
            await _mediator.Send(new CreateEscrowCommand
            {
                BookingId = booking.Id,
                WalletId = customerWallet.Id,
                Amount = booking.EstimatedPrice
            }, cancellationToken);
            await _mediator.Send(new CreateConversationCommandDTO { BookingId = booking.Id }, cancellationToken);

            return booking.Id;
        }
    }
}
