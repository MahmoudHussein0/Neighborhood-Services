using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;

namespace Neighborhood.Services.Application.Bookings.Queries.GetBookingsByStatusQuery
{
    public class GetBookingsByStatusQueryHandler : IRequestHandler<GetBookingsByStatusQuery, IEnumerable<BookingSummaryDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetBookingsByStatusQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<BookingSummaryDto>> Handle(GetBookingsByStatusQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetBookingsByStatusAsync(request.Status);

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
