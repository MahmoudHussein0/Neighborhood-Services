using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;

namespace Neighborhood.Services.Application.Bookings.Queries.GetTechnicianBusySlots
{
    public class GetTechnicianBusySlotsQueryHandler
        : IRequestHandler<GetTechnicianBusySlotsQuery, IEnumerable<BusySlotDto>>
    {
        private readonly IBookingRepository _bookingRepository;

        public GetTechnicianBusySlotsQueryHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<BusySlotDto>> Handle(
            GetTechnicianBusySlotsQuery request, CancellationToken cancellationToken)
        {
            var bookings = await _bookingRepository.GetConfirmedBookingsInRangeAsync(
                request.TechnicianId, request.From, request.To);

            // Each confirmed booking is one busy window [ScheduledAt, ScheduledAt + Duration).
            return bookings.Select(b => new BusySlotDto
            {
                Start = b.ScheduledAt,
                End = b.ScheduledAt.AddMinutes(b.DurationMinutes!.Value)
            }).ToList();
        }
    }
}
