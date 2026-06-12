using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.CompleteBookingCommands
{
    public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<CompleteBookingCommandHandler> _logger;

        public CompleteBookingCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, INotificationService notificationService, ILogger<CompleteBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            // Only the assigned technician can mark the booking Completed
            if (booking.Technician.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this booking.");

            if (booking.Status != BookingStatus.Confirmed)
                throw new BadRequestException($"Only a confirmed booking can be completed. Current status: {booking.Status}.");

            booking.Status = BookingStatus.Completed;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the customer the job is done and awaits their confirmation — best effort.
            try
            {
                if (!string.IsNullOrEmpty(booking.Customer?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        booking.Customer.ApplicationUserId,
                        $"Booking #{booking.Id} was marked complete. Please confirm to release the payment.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Complete-booking notification failed for booking {Id}.", booking.Id);
            }

            return true;
        }
    }
}
