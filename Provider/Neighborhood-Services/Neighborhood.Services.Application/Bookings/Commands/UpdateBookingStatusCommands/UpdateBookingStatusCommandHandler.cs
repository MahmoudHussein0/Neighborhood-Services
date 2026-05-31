using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Commands.UpdateBookingStatusCommands
{
    public class UpdateBookingStatusCommandHandler : IRequestHandler<UpdateBookingStatusCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookingStatusCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);
            // TODO: Add authorization check once current user service is ready
            // Only technician can confirm/complete
            // Customer or technician can cancel

            ValidateTransition(booking.Status, request.NewStatus);

            booking.Status = request.NewStatus;
            booking.UpdatedAt = DateTime.UtcNow;

            // TODO: when status moves to Confirmed, create Escrow to hold payment from customer wallet
            // coordinate with financials teammate

            await _bookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

         // completed and cancelled are not allowed to change anyway. only pending and confirmed are.
        private static void ValidateTransition(BookingStatus current, BookingStatus next)
        {
            var allowed = new Dictionary<BookingStatus, BookingStatus[]>
            {
                { BookingStatus.Pending,   new[] { BookingStatus.Confirmed, BookingStatus.Cancelled } },
                { BookingStatus.Confirmed, new[] { BookingStatus.Completed, BookingStatus.Cancelled } },
            };

            if (!allowed.TryGetValue(current, out var validNext) || !validNext.Contains(next))
                throw new BadRequestException($"Cannot transition booking from {current} to {next}.");
        }
    }
}
