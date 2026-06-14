using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.RejectQuoteCommands
{
    public class RejectQuoteCommandHandler : IRequestHandler<RejectQuoteCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<RejectQuoteCommandHandler> _logger;

        public RejectQuoteCommandHandler(
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            ILogger<RejectQuoteCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(RejectQuoteCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            if (booking.Customer.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this booking.");

            if (booking.Status != BookingStatus.Quoted)
                throw new BadRequestException($"Only a quoted booking can be rejected. Current status: {booking.Status}.");

            // Clear the rejected quote and bounce back so the tech can re-quote.
            booking.FinalPrice = 0;
            booking.DurationMinutes = null;
            booking.Status = BookingStatus.Pending;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the technician their quote was rejected — best effort.
            try
            {
                if (!string.IsNullOrEmpty(booking.Technician?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        booking.Technician.ApplicationUserId,
                        $"Your price quote for booking #{booking.Id} was rejected. You can submit a new quote.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Reject-quote notification failed for booking {Id}.", booking.Id);
            }

            return true;
        }
    }
}
