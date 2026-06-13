using Mapster;
using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.TechnitianAvailability.DTOs;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;


namespace Neighborhood.Services.Application.TechnitianAvailability.Queries
{
    public class GetTechAvailabilityForTechnicianQueryHandler : IRequestHandler<GetTechAvailabilityForTechnicianQuery, IReadOnlyList<TechnicianAvailabilityDetailsDTO>>
    {
        private readonly ITechnicianAvailabilityRepository _availabilityRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianRepository _technicianRepo;

        public GetTechAvailabilityForTechnicianQueryHandler(ITechnicianAvailabilityRepository availabilityRepo, ICurrentUserService currentUserService, ITechnicianRepository technicianRepo)
        {
            _availabilityRepo = availabilityRepo;
            _currentUserService = currentUserService;
            _technicianRepo = technicianRepo;
        }

        public async Task<IReadOnlyList<TechnicianAvailabilityDetailsDTO>> Handle(GetTechAvailabilityForTechnicianQuery request, CancellationToken cancellationToken)
        {

            string? userId = _currentUserService.UserId;

            if (userId is null)
                throw new UnauthorizedException("User not autherized");


            var technician = await _technicianRepo.GetByUserIdAsync(userId);

            if (technician is null)
                throw new ForbiddenException("User is not a technician");

            var techAvailabilities = await _availabilityRepo.GetByConditionAsync(TA => (!TA.IsDeleted) && TA.TechnicianId == technician.Id);

            if (techAvailabilities == null || !techAvailabilities.Any())

                return [];

            return techAvailabilities.OrderByDescending(TA => TA.DayOfWeek).ThenByDescending(TA => TA.StartTime).Adapt<IReadOnlyList<TechnicianAvailabilityDetailsDTO>>();

        }
    }
}

