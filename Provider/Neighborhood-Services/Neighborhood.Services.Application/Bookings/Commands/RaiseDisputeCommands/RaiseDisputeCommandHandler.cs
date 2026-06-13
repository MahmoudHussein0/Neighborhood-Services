using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Disputes.Commands;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.RaiseDisputeCommands
{
    public class RaiseDisputeCommandHandler : IRequestHandler<RaiseDisputeCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public RaiseDisputeCommandHandler(
            IBookingRepository bookingRepository,
            ICurrentUserService currentUserService,
            IMediator mediator)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<bool> Handle(RaiseDisputeCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new ValidationException("A reason is required to raise a dispute.");

            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);
            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            // Only the two parties to the booking can dispute it.
            if (booking.Customer.ApplicationUserId != userId && booking.Technician.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this booking.");

            // Already disputed — give a clear, specific message instead of the generic status error.
            if (booking.Status == BookingStatus.Disputed)
                throw new ConflictException("This booking already has an open dispute.");

            // A dispute only makes sense once there's a committed job: Confirmed or Completed.
            if (booking.Status != BookingStatus.Confirmed && booking.Status != BookingStatus.Completed)
                throw new BadRequestException($"A {booking.Status} booking can't be disputed.");

            // Flip the booking (our rule). Tracked, not yet saved.
            booking.Status = BookingStatus.Disputed;
            booking.UpdatedAt = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking);

            // Create the dispute record (Disputes module). Its SaveChanges commits BOTH the dispute
            // row and our tracked status change in one transaction. Its "one dispute per booking"
            // guard throws before saving if a dispute already exists → our flip rolls back. Atomic.
            await _mediator.Send(new CreateDisputeCommand
            {
                BookingId = booking.Id,
                RaisedByUserId = userId,
                DisputeType = request.DisputeType,
                Reason = request.Reason
            }, cancellationToken);

            return true;
        }
    }
}
