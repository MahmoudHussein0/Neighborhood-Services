using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.StaffCancelBookingCommands
{
    public class StaffCancelBookingCommandHandler : IRequestHandler<StaffCancelBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<StaffCancelBookingCommandHandler> _logger;

        public StaffCancelBookingCommandHandler(
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            ILogger<StaffCancelBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(StaffCancelBookingCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.CancellationReason))
                throw new ValidationException("A cancellation reason is required.");

            var staffUserId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId)
                ?? throw new NotFoundException(nameof(Booking), request.BookingId);

            // Can't cancel something already finished, already cancelled, or under dispute
            // (dispute resolution is a separate flow — not this oversight cancel).
            if (booking.Status is BookingStatus.Completed or BookingStatus.Cancelled or BookingStatus.Disputed)
                throw new BadRequestException($"A {booking.Status} booking can't be cancelled by staff.");

            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = request.CancellationReason;
            booking.CancelledBy = staffUserId;
            booking.CancelledAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            // NOTE: no escrow refund here by design. If money is held, it must be handled
            // separately (refund/dispute flow). This command only changes booking state.

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify both parties that staff cancelled the booking — best effort.
            try
            {
                var message = $"Booking #{booking.Id} was cancelled by staff. Reason: {booking.CancellationReason}";

                if (!string.IsNullOrEmpty(booking.Customer?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(booking.Customer.ApplicationUserId, message);

                if (!string.IsNullOrEmpty(booking.Technician?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(booking.Technician.ApplicationUserId, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Staff-cancel-booking notification failed for booking {Id}.", booking.Id);
            }

            return true;
        }
    }
}
