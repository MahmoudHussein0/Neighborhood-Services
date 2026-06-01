using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.CancellationPolicies.Interfaces;
using Neighborhood.Services.Application.Escrows.Commands.RefundEscrow;
using Neighborhood.Services.Application.Escrows.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.CancellationPolicies;
using Neighborhood.Services.Domain.Escrows;

namespace Neighborhood.Services.Application.Bookings.Commands.CancelBookingCommands
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICancellationPolicyRepository _cancellationPolicyRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public CancelBookingCommandHandler(
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            ICancellationPolicyRepository cancellationPolicyRepository,
            IEscrowRepository escrowRepository,
            IMediator mediator,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _cancellationPolicyRepository = cancellationPolicyRepository;
            _escrowRepository = escrowRepository;
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);

            // checking if booking exists
            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            // Authorization: only the customer or technician on this booking can cancel it
            var cancelledByCustomer = booking.Customer.ApplicationUserId == userId;
            var cancelledByTechnician = booking.Technician.ApplicationUserId == userId;
            if (!cancelledByCustomer && !cancelledByTechnician)
                throw new ForbiddenException("You don't have access to this booking.");

            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Completed)
                throw new BadRequestException($"Booking cannot be cancelled because it is already {booking.Status}.");

            // Determine who is cancelling, to pick the right cancellation policy target
            var target = cancelledByCustomer
                ? CancellationPolicyTarget.Customer
                : CancellationPolicyTarget.Technican;

            //checking if there is penality and calculate it 
            var hoursBeforeBooking = (booking.ScheduledAt - DateTime.UtcNow).TotalHours;
            var policy = await _cancellationPolicyRepository
            .GetPolicyAsync((int)hoursBeforeBooking, target);
            if (policy != null && policy.PenaltyPct > 0)
            {
                var escrow = await _escrowRepository.GetByBookingIdAsync(booking.Id);

                // TODO: Deduct penalty from escrow before refunding remainder
                // penalty amount = escrow.Amount * policy.PenaltyPct / 100
                // Coordinate with Ziad (financials) — needs partial refund support
            }
            else
            {
                // No penalty — full refund if escrow exists and is still held
                var escrow = await _escrowRepository.GetByBookingIdAsync(booking.Id);
                if (escrow is not null && escrow.Status == EscrowStatus.Held)
                    await _mediator.Send(new RefundEscrowCommand { EscrowId = escrow.Id }, cancellationToken);
            }


            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = request.CancellationReason;
            booking.CancelledBy = userId;
            booking.CancelledAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
