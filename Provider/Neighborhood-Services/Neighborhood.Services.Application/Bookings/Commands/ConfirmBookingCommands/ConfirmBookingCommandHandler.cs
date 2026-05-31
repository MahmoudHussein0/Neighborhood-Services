using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.ConfirmBookingCommands
{
    public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmBookingCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            if (booking.Status != BookingStatus.Completed)
                throw new BadRequestException("Booking can only be confirmed by the client after it is marked Completed by the technician.");

            if (booking.ClientConfirmed)
                throw new BadRequestException("Booking has already been confirmed by the client.");

            booking.ClientConfirmed = true;
            booking.ConfirmedAt = DateTime.UtcNow;
            booking.UpdatedAt = DateTime.UtcNow;

            // TODO: release escrow funds to technician wallet
            // coordinate with financials teammate
            // TODO: Release escrow funds to technician wallet
            // When ClientConfirmed = true:
            // 1. Get Escrow by BookingId
            // 2. Transfer amount from Escrow to Technician wallet
            // 3. Update Escrow status to Released
            // Coordinate with Mahmoud (financials)

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
