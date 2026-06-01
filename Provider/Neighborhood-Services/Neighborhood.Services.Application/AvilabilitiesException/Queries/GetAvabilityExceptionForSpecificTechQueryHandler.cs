using Mapster;
using MediatR;
using Neighborhood.Services.Application.AvilabilitiesException.DTOs;
using Neighborhood.Services.Application.AvilabilitiesException.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.AvilabilitiesException.Queries
{
    public class GetAvabilityExceptionForSpecificTechQueryHandler : IRequestHandler<GetAvabilityExceptionForSpecificTechQuery, IReadOnlyList<AvailiabilityExceptionDTO>>
    {

        private readonly IAvailabilityExceptionRepository _exceptionRepo;

        public GetAvabilityExceptionForSpecificTechQueryHandler(IAvailabilityExceptionRepository exceptionRepo)
        {
            _exceptionRepo = exceptionRepo;
        }
        public async Task<IReadOnlyList<AvailiabilityExceptionDTO>> Handle(GetAvabilityExceptionForSpecificTechQuery request, CancellationToken cancellationToken)
        {

          var techAvabilityException =  await _exceptionRepo.GetByConditionAsync( AE => AE.TechnicianId == request.TechnicianId );

            if (techAvabilityException is null || !techAvabilityException.Any()) 
            {
                throw new ValidationException(new Dictionary<string, string[]>
                { { "TechnicianId", new[] { "No exception found for this technician." } }});}

            return techAvabilityException.OrderByDescending(AE => AE.Date).Adapt<IReadOnlyList<AvailiabilityExceptionDTO>>();


        }
    }
}
