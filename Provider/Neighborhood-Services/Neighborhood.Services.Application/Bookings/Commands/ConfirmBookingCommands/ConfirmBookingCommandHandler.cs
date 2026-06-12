using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Escrows.Commands.ReleaseEscrow;
using Neighborhood.Services.Application.Escrows.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.Escrows;

namespace Neighborhood.Services.Application.Bookings.Commands.ConfirmBookingCommands
{
    public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IEscrowRepository _escrowRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ConfirmBookingCommandHandler> _logger;

        public ConfirmBookingCommandHandler(
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            IEscrowRepository escrowRepository,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            ILogger<ConfirmBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _escrowRepository = escrowRepository;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            // Only the customer who owns this booking can confirm it
            if (booking.Customer.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this booking.");

            if (booking.Status != BookingStatus.Completed)
                throw new BadRequestException("Booking can only be confirmed by the client after it is marked Completed by the technician.");

            if (booking.ClientConfirmed)
                throw new BadRequestException("Booking has already been confirmed by the client.");

            booking.ClientConfirmed = true;
            booking.ConfirmedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            var escrow = await _escrowRepository.GetByBookingIdAsync(booking.Id);
            if (escrow is not null && escrow.Status == EscrowStatus.Held)
                await _mediator.Send(new ReleaseEscrowCommand { EscrowId = escrow.Id }, cancellationToken);

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the technician the customer confirmed and payment was released — best effort.
            try
            {
                if (!string.IsNullOrEmpty(booking.Technician?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        booking.Technician.ApplicationUserId,
                        $"The customer confirmed booking #{booking.Id}. Your payment has been released.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Confirm-booking notification failed for booking {Id}.", booking.Id);
            }

            return true;
        }
    }
}
