using Mapster;
using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianAvailability.DTOs;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;


namespace Neighborhood.Services.Application.TechnitianAvailability.Queries
{
    public class GetTechAvailabilityForTechnicianQueryHandler : IRequestHandler<GetTechAvailabilityForTechnicianQuery, IReadOnlyList<TechnicianAvailabilityDetailsDTO>>
    {
        private readonly ITechnicianAvailabilityRepository _availabilityRepo;

        public GetTechAvailabilityForTechnicianQueryHandler(ITechnicianAvailabilityRepository availabilityRepo)
        {
            _availabilityRepo = availabilityRepo;
        }

        public async Task<IReadOnlyList<TechnicianAvailabilityDetailsDTO>> Handle(GetTechAvailabilityForTechnicianQuery request, CancellationToken cancellationToken)
        {

          var techAvailabilities =  await _availabilityRepo.GetByConditionAsync(TA =>  (!TA.IsDeleted) &&  TA.TechnicianId == request.TechnicianId );
            
            if (techAvailabilities == null || !techAvailabilities.Any())
            {
                throw new ValidationException("No availability found for this technician."); }


          return  techAvailabilities.OrderByDescending(TA => TA.DayOfWeek).ThenByDescending(TA => TA.StartTime).Adapt<IReadOnlyList<TechnicianAvailabilityDetailsDTO>>();

        }
    }
}

