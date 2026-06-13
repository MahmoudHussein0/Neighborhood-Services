using Mapster;
using MediatR;
using Neighborhood.Services.Application.TechnitianAvailability.DTOs;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;

namespace Neighborhood.Services.Application.TechnitianAvailability.Queries
{
    public class GetTechAvailabilityByTechnicianIdQueryHandler : IRequestHandler<GetTechAvailabilityByTechnicianIdQuery, IReadOnlyList<TechnicianAvailabilityDetailsDTO>>
    {
        private readonly ITechnicianAvailabilityRepository _availabilityRepo;

        public GetTechAvailabilityByTechnicianIdQueryHandler(ITechnicianAvailabilityRepository availabilityRepo)
        {
            _availabilityRepo = availabilityRepo;
        }

        public async Task<IReadOnlyList<TechnicianAvailabilityDetailsDTO>> Handle(GetTechAvailabilityByTechnicianIdQuery request, CancellationToken cancellationToken)
        {
            var techAvailabilities = await _availabilityRepo.GetByConditionAsync(
                TA => (!TA.IsDeleted) && TA.TechnicianId == request.TechnicianId);

            if (techAvailabilities == null || !techAvailabilities.Any())
                return [];

            return techAvailabilities
                .OrderByDescending(TA => TA.DayOfWeek)
                .ThenByDescending(TA => TA.StartTime)
                .Adapt<IReadOnlyList<TechnicianAvailabilityDetailsDTO>>();
        }
    }
}
