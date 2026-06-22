using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Conversations.Commands;
using Neighborhood.Services.Application.Escrows.Commands.CreateEscrow;
using Neighborhood.Services.Application.Messages.Commands;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.Offers.Interfaces;
using Neighborhood.Services.Application.PromoCodes.Commands.ApplyPromoCode;
using Neighborhood.Services.Application.PromoCodes.Interface;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
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
        private readonly ITechnicianRepository _technicianRepository;
        private readonly INotificationService _notificationService;
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly IPromoCodeUsageRepository _promoCodeUsageRepository;
        private readonly ILogger<AcceptOfferCommandHandler> _logger;

        public AcceptOfferCommandHandler(
            IOfferRepository offerRepository,
            IServiceRequestRepository serviceRequestRepository,
            IBookingRepository bookingRepository,
            IWalletRepository walletRepository,
            ICurrentUserService currentUserService,
            IMediator mediator,
            IUnitOfWork unitOfWork,
            ITechnicianRepository technicianRepository,
            INotificationService notificationService,
            IPromoCodeRepository promoCodeRepository,
            IPromoCodeUsageRepository promoCodeUsageRepository,
            ILogger<AcceptOfferCommandHandler> logger)
        {
            _offerRepository = offerRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _bookingRepository = bookingRepository;
            _walletRepository = walletRepository;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _technicianRepository = technicianRepository;
            _notificationService = notificationService;
            _promoCodeRepository = promoCodeRepository;
            _promoCodeUsageRepository = promoCodeUsageRepository;
            _logger = logger;
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

            // Price the (optional) promo up front so the balance check below reflects what the
            // customer will actually pay, and so an invalid/expired/used/maxed code fails fast
            // BEFORE the booking is created. The authoritative discount + usage recording still
            // happens in ApplyPromoCode after the booking is saved (single place that mutates).
            var effectivePrice = offer.Price;
            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                var code = request.PromoCode.Trim();

                if (!await _promoCodeRepository.IsValidAsync(code))
                    throw new BadRequestException($"Promo code '{code}' is invalid or expired.");

                var promo = await _promoCodeRepository.GetByCodeAsync(code)
                    ?? throw new BadRequestException($"Promo code '{code}' not found.");

                if (await _promoCodeUsageRepository.HasUserUsedPromoAsync(userId, promo.Id))
                    throw new BadRequestException("You have already used this promo code.");

                var discount = Math.Round(offer.Price * promo.DiscountPercentage / 100, 2);
                effectivePrice = Math.Max(0, offer.Price - discount);
            }

            // Fail fast on insufficient funds before creating the booking, so the
            // escrow step (run after the booking is saved) is very unlikely to fail.
            var customerWallet = await _walletRepository.GetByUserIdAsync(serviceRequest.Customer.ApplicationUserId)
                ?? throw new NotFoundException("Wallet", serviceRequest.Customer.ApplicationUserId);
            if (customerWallet.Balance < effectivePrice)
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
                // FinalPrice is the canonical "what the customer pays" — seeded from the offer
                // and discounted below if a promo code is supplied. Escrow is held at FinalPrice.
                FinalPrice = offer.Price,
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

            // Optional promo code: now that the booking is persisted (has an Id), discount its
            // FinalPrice and record usage atomically before the escrow is held below.
            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                await _mediator.Send(new ApplyPromoCodeCommand
                {
                    Code = request.PromoCode,
                    UserId = userId,
                    BookingId = booking.Id
                }, cancellationToken);
            }

            // Hold the customer's payment in escrow (manages its own transaction).
            await _mediator.Send(new CreateEscrowCommand
            {
                BookingId = booking.Id,
                WalletId = customerWallet.Id,
                Amount = booking.FinalPrice
            }, cancellationToken);
            await _mediator.Send(new CreateConversationCommandDTO { BookingId = booking.Id }, cancellationToken);

            // Seed a short greeting from each side so the conversation shows up for both parties
            // with the other person's name/avatar populated (best effort — never blocks the accept).
            try
            {
                var seedTech = await _technicianRepository.GetByIdAsync(offer.TechnicianId);

                if (!string.IsNullOrEmpty(serviceRequest.Customer?.ApplicationUserId))
                    await _mediator.Send(new CreateMessageCommand
                    {
                        BookingId = booking.Id,
                        SenderId = serviceRequest.Customer.ApplicationUserId,
                        content = "Hi 👋"
                    }, cancellationToken);

                if (!string.IsNullOrEmpty(seedTech?.ApplicationUserId))
                    await _mediator.Send(new CreateMessageCommand
                    {
                        BookingId = booking.Id,
                        SenderId = seedTech.ApplicationUserId,
                        content = "Hello 👋"
                    }, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Seeding chat starter messages failed for booking {Id}.", booking.Id);
            }

            // Booking confirmed + conversation opened — notify both parties about the chat (best effort).
            try
            {
                var technician = await _technicianRepository.GetByIdAsync(offer.TechnicianId);
                if (!string.IsNullOrEmpty(technician?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        technician.ApplicationUserId,
                        $"Your offer on service request #{serviceRequest.Id} was accepted. Booking #{booking.Id} is confirmed — you can now chat with the customer.");

                if (!string.IsNullOrEmpty(serviceRequest.Customer?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        serviceRequest.Customer.ApplicationUserId,
                        $"Booking #{booking.Id} is confirmed. You can now chat with your technician.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Accept-offer notification failed for offer {Id}.", offer.Id);
            }

            return booking.Id;
        }
    }
}
