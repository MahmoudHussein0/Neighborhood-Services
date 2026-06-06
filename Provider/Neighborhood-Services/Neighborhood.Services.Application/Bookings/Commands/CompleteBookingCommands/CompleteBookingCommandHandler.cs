using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.CompleteBookingCommands
{
    public class CompleteBookingCommandHandler : IRequestHandler<CompleteBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CompleteBookingCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
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

            return true;
        }
    }
}
