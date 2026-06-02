using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingsByCustomerQuery
{
    public class GetBookingsByCustomerQueryHandler : IRequestHandler<GetBookingsByCustomerQuery, IEnumerable<BookingSummaryDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingsByCustomerQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<BookingSummaryDto>> Handle(GetBookingsByCustomerQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetCustomerBookingsAsync(request.CustomerId);

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
