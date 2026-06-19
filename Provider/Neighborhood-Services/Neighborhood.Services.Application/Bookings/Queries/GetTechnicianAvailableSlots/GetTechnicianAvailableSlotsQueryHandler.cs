using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;

namespace Neighborhood.Services.Application.Bookings.Queries.GetTechnicianAvailableSlots
{
    public class GetTechnicianAvailableSlotsQueryHandler
        : IRequestHandler<GetTechnicianAvailableSlotsQuery, IEnumerable<DateTime>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITechnicianAvailabilityRepository _availabilityRepository;

        public GetTechnicianAvailableSlotsQueryHandler(
            IBookingRepository bookingRepository,
            ITechnicianAvailabilityRepository availabilityRepository)
        {
            _bookingRepository = bookingRepository;
            _availabilityRepository = availabilityRepository;
        }

        public async Task<IEnumerable<DateTime>> Handle(
            GetTechnicianAvailableSlotsQuery request, CancellationToken cancellationToken)
        {
            var date = request.Date.Date;
            var step = request.SlotMinutes <= 0 ? 30 : request.SlotMinutes;

            // 1. The tech's working window(s) for that weekday. No availability → no slots.
            var windows = (await _availabilityRepository.GetByConditionAsync(
                a => a.TechnicianId == request.TechnicianId && a.DayOfWeek == date.DayOfWeek)).ToList();

            if (windows.Count == 0)
                return Enumerable.Empty<DateTime>();

            // 2. The tech's busy windows for that day (confirmed bookings, with their durations).
            var busy = (await _bookingRepository.GetConfirmedBookingsInRangeAsync(
                    request.TechnicianId, date, date.AddDays(1)))
                .Select(b => (Start: b.ScheduledAt, End: b.ScheduledAt.AddMinutes(b.DurationMinutes!.Value)))
                .ToList();

            // 3. Walk each working window in `step` increments, dropping past slots and any start
            //    that falls inside a busy window.
            var now = DateTime.UtcNow;
            var slots = new List<DateTime>();

            foreach (var w in windows)
            {
                var start = date + w.StartTime.ToTimeSpan();
                var end = date + w.EndTime.ToTimeSpan();

                for (var t = start; t < end; t = t.AddMinutes(step))
                {
                    if (t <= now) continue;
                    if (busy.Any(b => b.Start <= t && t < b.End)) continue;
                    slots.Add(t);
                }
            }

            return slots.Distinct().OrderBy(t => t).ToList();
        }
    }
}
