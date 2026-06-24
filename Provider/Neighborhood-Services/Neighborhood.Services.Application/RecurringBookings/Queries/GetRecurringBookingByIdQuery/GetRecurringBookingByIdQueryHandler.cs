using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.RecurringBookings.DTOs;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Domain.RecurringBookings;

namespace Neighborhood.Services.Application.RecurringBookings.Queries.GetRecurringBookingByIdQuery
{
    public class GetRecurringBookingByIdQueryHandler : IRequestHandler<GetRecurringBookingByIdQuery, RecurringBookingDto>
    {
        private readonly IRecurringBookingRepository _repository;

        public GetRecurringBookingByIdQueryHandler(IRecurringBookingRepository repository)
        {
            _repository = repository;
        }

        public async Task<RecurringBookingDto> Handle(GetRecurringBookingByIdQuery request, CancellationToken cancellationToken)
        {
            var rb = await _repository.GetRecurringBookingWithDetailsAsync(request.RecurringBookingId);
            if (rb is null)
                throw new NotFoundException(nameof(RecurringBooking), request.RecurringBookingId);

            return MapToDto(rb);
        }

        internal static RecurringBookingDto MapToDto(RecurringBooking rb) => new()
        {
            Id = rb.Id,
            Description = rb.Description,
            ImageUrl = rb.ImageUrl,
            Address = rb.Address,
            Pattern = rb.Pattern,
            DayOfWeek = rb.DayOfWeek,
            DayOfMonth = rb.DayOfMonth,
            TimeOfDay = rb.TimeOfDay,
            DurationMinutes = rb.DurationMinutes,
            StartDate = rb.StartDate,
            EndDate = rb.EndDate,
            Status = rb.Status,
            AgreedPrice = rb.AgreedPrice,
            CustomerId = rb.CustomerId,
            TechnicianId = rb.TechnicianId,
            ProblemTypeId = rb.ProblemTypeId,
            ProblemTypeNameEn = rb.ProblemType != null ? rb.ProblemType.NameEn : string.Empty,
            ProblemTypeNameAr = rb.ProblemType != null ? rb.ProblemType.NameAr : string.Empty,
            CreatedAt = rb.CreatedAt
        };
    }
}
