using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.StaffCancelBookingCommands
{
    public class StaffCancelBookingCommandHandler : IRequestHandler<StaffCancelBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public StaffCancelBookingCommandHandler(
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(StaffCancelBookingCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.CancellationReason))
                throw new ValidationException("A cancellation reason is required.");

            var staffUserId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetByIdAsync(request.BookingId)
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

            return true;
        }
    }
}
