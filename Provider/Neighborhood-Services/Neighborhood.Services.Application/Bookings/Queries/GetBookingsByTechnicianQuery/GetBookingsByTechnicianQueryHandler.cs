using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingsByTechnicianQuery
{
    public class GetBookingsByTechnicianQueryHandler : IRequestHandler<GetBookingsByTechnicianQuery, IEnumerable<BookingSummaryDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingsByTechnicianQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<BookingSummaryDto>> Handle(GetBookingsByTechnicianQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetTechnicianBookingsAsync(request.TechnicianId);

            return bookings.Select(b => new BookingSummaryDto
            {
                Id = b.Id,
                BookingType = b.BookingType,
                Description = b.Description,
                Address = b.Address,
                ScheduledAt = b.ScheduledAt,
                EstimatedPrice = b.EstimatedPrice,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            });
        }
    }
}
