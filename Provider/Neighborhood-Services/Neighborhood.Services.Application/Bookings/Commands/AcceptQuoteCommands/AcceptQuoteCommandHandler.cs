using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Conversations.Commands;
using Neighborhood.Services.Application.Messages.Commands;
using Neighborhood.Services.Application.Escrows.Commands.CreateEscrow;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.PromoCodes.Commands.ApplyPromoCode;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.AcceptQuoteCommands
{
    public class AcceptQuoteCommandHandler : IRequestHandler<AcceptQuoteCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AcceptQuoteCommandHandler> _logger;

        public AcceptQuoteCommandHandler(
            IBookingRepository bookingRepository,
            IWalletRepository walletRepository,
            IMediator mediator,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            ILogger<AcceptQuoteCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _walletRepository = walletRepository;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(AcceptQuoteCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            // Only the customer who owns this booking can accept the quote
            if (booking.Customer.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this booking.");

            if (booking.Status != BookingStatus.Quoted)
                throw new BadRequestException($"Only a quoted booking can be accepted. Current status: {booking.Status}.");

            if (booking.FinalPrice <= 0)
                throw new BadRequestException("Booking has no quoted price.");

            // Make sure the tech is still free for the quoted duration
            var start = booking.ScheduledAt;
            var end = start.AddMinutes(booking.DurationMinutes ?? 0);
            var hasOverlap = await _bookingRepository.HasOverlappingConfirmedBookingAsync(
                booking.TechnicianId, start, end, booking.Id);
            if (hasOverlap)
                throw new ConflictException("Technician already has a confirmed booking that overlaps this time.");

            // Optional promo code: discounts booking.FinalPrice (and records usage) atomically,
            // so the escrow below is held at the discounted amount. Same EF-tracked booking instance.
            if (!string.IsNullOrWhiteSpace(request.PromoCode))
            {
                await _mediator.Send(new ApplyPromoCodeCommand
                {
                    Code = request.PromoCode,
                    UserId = userId,
                    BookingId = booking.Id
                }, cancellationToken);
            }

            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;

            // Hold the customer's payment in escrow at the quoted FinalPrice
            var customerWallet = await _walletRepository.GetByUserIdAsync(booking.Customer.ApplicationUserId)
                ?? throw new NotFoundException("Wallet", booking.Customer.ApplicationUserId);

            await _mediator.Send(new CreateEscrowCommand
            {
                BookingId = booking.Id,
                WalletId = customerWallet.Id,
                Amount = booking.FinalPrice
            }, cancellationToken);

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _mediator.Send(new CreateConversationCommandDTO { BookingId = booking.Id }, cancellationToken);

            // Seed a short greeting from each side so the conversation shows up for both parties
            // with the other person's name/avatar populated (best effort — never blocks the accept).
            try
            {
                if (!string.IsNullOrEmpty(booking.Customer?.ApplicationUserId))
                    await _mediator.Send(new CreateMessageCommand
                    {
                        BookingId = booking.Id,
                        SenderId = booking.Customer.ApplicationUserId,
                        content = "Hi 👋"
                    }, cancellationToken);

                if (!string.IsNullOrEmpty(booking.Technician?.ApplicationUserId))
                    await _mediator.Send(new CreateMessageCommand
                    {
                        BookingId = booking.Id,
                        SenderId = booking.Technician.ApplicationUserId,
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
                if (!string.IsNullOrEmpty(booking.Technician?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        booking.Technician.ApplicationUserId,
                        $"Your quote for booking #{booking.Id} was accepted. The job is confirmed — you can now chat with the customer.");

                if (!string.IsNullOrEmpty(booking.Customer?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        booking.Customer.ApplicationUserId,
                        $"Booking #{booking.Id} is confirmed. You can now chat with your technician.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Accept-quote notification failed for booking {Id}.", booking.Id);
            }

            return true;
        }
    }
}
