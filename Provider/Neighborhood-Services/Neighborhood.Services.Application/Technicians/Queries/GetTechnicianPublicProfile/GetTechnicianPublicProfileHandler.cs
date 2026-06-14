using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.PublicProfiles.DTOs;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetTechnicianPublicProfileHandler(ITechnicianRepository technicianRepository)
        : IRequestHandler<GetTechnicianPublicProfileQuery, PublicProfileDto>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task<PublicProfileDto> Handle(GetTechnicianPublicProfileQuery request, CancellationToken cancellationToken)
        {
            return await _technicianRepository.GetPublicProfileAsync(request.TechnicianId)
                ?? throw new NotFoundException("Technician", request.TechnicianId);
        }
    }
}
