using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.CancellationPolicies.Interfaces;
using Neighborhood.Services.Application.Escrows.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.CancellationPolicies;

namespace Neighborhood.Services.Application.Bookings.Commands.CancelBookingCommands
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICancellationPolicyRepository _cancellationPolicyRepository;
        private readonly IEscrowRepository _escrowRepository;

        public CancelBookingCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork, ICancellationPolicyRepository cancellationPolicyRepository , IEscrowRepository escrowRepository)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _cancellationPolicyRepository= cancellationPolicyRepository;
            _escrowRepository = escrowRepository;
        }

        public async Task<bool> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);
            // TODO: Authorization check once current user service is ready
            // Verify requesting user is either the customer or technician of this booking
            // if (booking.Customer.UserId != requestingUserId && booking.Technician.UserId != requestingUserId)
            //     throw new ForbiddenException("You don't have access to this booking");

            // checking if booking exists 
            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            if (booking.Status == BookingStatus.Cancelled || booking.Status == BookingStatus.Completed)
                throw new BadRequestException($"Booking cannot be cancelled because it is already {booking.Status}.");
            // getting who cancelled it ?
            // Determine who is cancelling
            var cancelledByCustomer = booking.Customer.ApplicationUserId == request.CancelledBy;
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

                // TODO: Deduct penalty from escrow
                // penalty amount = escrow.Amount * policy.PenaltyPct / 100
                // Coordinate with (financials)
            }


            booking.Status = BookingStatus.Cancelled;
            booking.CancellationReason = request.CancellationReason;
            booking.CancelledBy = request.CancelledBy;
            booking.CancelledAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
