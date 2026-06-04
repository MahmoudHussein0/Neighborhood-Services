using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Conversations.Commands;
using Neighborhood.Services.Application.Escrows.Commands.CreateEscrow;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.AcceptBookingCommands
{
    public class AcceptBookingCommandHandler : IRequestHandler<AcceptBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AcceptBookingCommandHandler(
            IBookingRepository bookingRepository,
            IWalletRepository walletRepository,
            IMediator mediator,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _walletRepository = walletRepository;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(AcceptBookingCommand request, CancellationToken cancellationToken)
        {
            if (request.DurationMinutes <= 0)
                throw new ValidationException("Duration must be greater than zero.");

            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            // Only the assigned technician can accept this booking
            if (booking.Technician.ApplicationUserId != userId)
                throw new ForbiddenException("You don't have access to this booking.");

            if (booking.Status != BookingStatus.Pending)
                throw new BadRequestException($"Only a pending booking can be accepted. Current status: {booking.Status}.");

            var start = booking.ScheduledAt;
            var end = start.AddMinutes(request.DurationMinutes);

            var hasOverlap = await _bookingRepository.HasOverlappingConfirmedBookingAsync(
                booking.TechnicianId, start, end, booking.Id);

            if (hasOverlap)
                throw new ConflictException("Technician already has a confirmed booking that overlaps this time.");

            booking.DurationMinutes = request.DurationMinutes;
            booking.Status = BookingStatus.Confirmed;
            booking.UpdatedAt = DateTime.UtcNow;

            // Hold the customer's payment in escrow now that the booking is confirmed
            var customerWallet = await _walletRepository.GetByUserIdAsync(booking.Customer.ApplicationUserId)
                ?? throw new NotFoundException("Wallet", booking.Customer.ApplicationUserId);

            await _mediator.Send(new CreateEscrowCommand
            {
                BookingId = booking.Id,
                WalletId = customerWallet.Id,
                Amount = booking.EstimatedPrice
            }, cancellationToken);

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _mediator.Send(new CreateConversationCommandDTO { BookingId=booking.Id}, cancellationToken);
            return true;
        }
    }
}
