using Mapster;
using MediatR;
using Neighborhood.Services.Application.AvilabilitiesException.DTOs;
using Neighborhood.Services.Application.AvilabilitiesException.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.AvilabilitiesException.Queries
{
    public class GetAvabilityExceptionForSpecificTechQueryHandler : IRequestHandler<GetAvabilityExceptionForSpecificTechQuery, IReadOnlyList<AvailiabilityExceptionDTO>>
    {

        private readonly IAvailabilityExceptionRepository _exceptionRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianRepository _technicianRepo;

        public GetAvabilityExceptionForSpecificTechQueryHandler(IAvailabilityExceptionRepository exceptionRepo , ICurrentUserService currentUserService, ITechnicianRepository technicianRepo)
        {
            _exceptionRepo = exceptionRepo;
           _currentUserService = currentUserService;
           _technicianRepo = technicianRepo;
        }
        public async Task<IReadOnlyList<AvailiabilityExceptionDTO>> Handle(GetAvabilityExceptionForSpecificTechQuery request, CancellationToken cancellationToken)
        {

            string? userId = _currentUserService.UserId;


            if (userId is null)
                throw new UnauthorizedException("User not autherized");


            var technician = await _technicianRepo.GetByUserIdAsync(userId);

            if (technician is null)
                throw new ForbiddenException("User is not a technician");

            var techAvabilityException =  await _exceptionRepo.GetByConditionAsync( AE => (!AE.IsDeleted) &&  AE.TechnicianId == technician.Id );

            if (techAvabilityException is null || !techAvabilityException.Any()) 
            { return []; }

            return techAvabilityException.OrderByDescending(AE => AE.Date).Adapt<IReadOnlyList<AvailiabilityExceptionDTO>>();


        }
    }
}
