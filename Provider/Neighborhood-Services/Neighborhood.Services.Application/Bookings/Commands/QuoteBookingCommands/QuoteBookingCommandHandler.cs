using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianPricing.Interface;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.QuoteBookingCommands
{
    public class QuoteBookingCommandHandler : IRequestHandler<QuoteBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITechnicianPricingRepository _technicianPricingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        private readonly ILogger<QuoteBookingCommandHandler> _logger;

        public QuoteBookingCommandHandler(
            IBookingRepository bookingRepository,
            ITechnicianPricingRepository technicianPricingRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            INotificationService notificationService,
            ILogger<QuoteBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _technicianPricingRepository = technicianPricingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(QuoteBookingCommand request, CancellationToken cancellationToken)
        {
            if (request.DurationMinutes <= 0)
                throw new ValidationException("Duration must be greater than zero.");

            if (request.FinalPrice <= 0)
                throw new ValidationException("Price must be greater than zero.");

            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            // Only the assigned technician can quote on this booking
            if (booking.Technician.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this booking.");

            // Tech is allowed to (re-)quote when it's their turn:
            //  * Pending — first quote, or customer rejected the previous quote
            //  * Quoted — tech is updating their own quote before the customer acts on it
            if (booking.Status != BookingStatus.Pending && booking.Status != BookingStatus.Quoted)
                throw new BadRequestException($"Cannot quote a booking that is {booking.Status}.");

            // Enforce: FinalPrice must fall within the tech's configured range for this problem type
            var pricingRows = await _technicianPricingRepository.GetByConditionAsync(
                tp => !tp.IsDeleted
                    && tp.TechnicianId == booking.TechnicianId
                    && tp.ProblemTypeId == booking.ProblemTypeId);

            var pricing = pricingRows.FirstOrDefault()
                ?? throw new BadRequestException("You haven't set a price range for this problem type yet.");

            if (request.FinalPrice < pricing.MinPrice || request.FinalPrice > pricing.MaxPrice)
                throw new ValidationException(
                    $"Price must be between {pricing.MinPrice} and {pricing.MaxPrice} for this problem type.");

            // Make sure the tech still has the slot free for the proposed duration
            var start = booking.ScheduledAt;
            var end = start.AddMinutes(request.DurationMinutes);
            var hasOverlap = await _bookingRepository.HasOverlappingConfirmedBookingAsync(
                booking.TechnicianId, start, end, booking.Id);
            if (hasOverlap)
                throw new ConflictException("Technician already has a confirmed booking that overlaps this time.");

            booking.FinalPrice = request.FinalPrice;
            booking.DurationMinutes = request.DurationMinutes;
            booking.Status = BookingStatus.Quoted;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the customer that a quote arrived — best effort.
            try
            {
                if (!string.IsNullOrEmpty(booking.Customer?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        booking.Customer.ApplicationUserId,
                        $"A technician sent a price quote for your booking #{booking.Id}.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Quote notification failed for booking {Id}.", booking.Id);
            }

            return true;
        }
    }
}
