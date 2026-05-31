using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingByIdQuery
{
    public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, BookingDetailsDto>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingByIdQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<BookingDetailsDto> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
        {
            var booking = await _bookingRepository.GetBookingWithDetailsAsync(request.BookingId);

            if (booking is null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            return new BookingDetailsDto
            {
                Id = booking.Id,
                BookingType = booking.BookingType,
                Description = booking.Description,
                Address = booking.Address,
                ScheduledAt = booking.ScheduledAt,
                EstimatedPrice = booking.EstimatedPrice,
                FinalPrice = booking.FinalPrice,
                Status = booking.Status,
                ClientConfirmed = booking.ClientConfirmed,
                CancellationReason = booking.CancellationReason,
                CreatedAt = booking.CreatedAt,
                CustomerId = booking.CustomerId,
                TechnicianId = booking.TechnicianId,
                ProblemTypeId = booking.ProblemTypeId,
                OfferId = booking.OfferId,
                ServiceRequestId = booking.ServiceRequestId
            };
        }
    }
}
