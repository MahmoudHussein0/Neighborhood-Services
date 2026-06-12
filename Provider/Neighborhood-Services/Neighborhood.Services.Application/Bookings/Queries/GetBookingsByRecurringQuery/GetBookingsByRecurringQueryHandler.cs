using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingsByRecurringQuery
{
    public class GetBookingsByRecurringQueryHandler : IRequestHandler<GetBookingsByRecurringQuery, IEnumerable<BookingSummaryDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingsByRecurringQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<BookingSummaryDto>> Handle(GetBookingsByRecurringQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetByRecurringBookingIdAsync(request.RecurringBookingId);

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
