using MediatR;
using Neighborhood.Services.Application.PublicProfiles.DTOs;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetTechnicianPublicProfileQuery : IRequest<PublicProfileDto>
    {
        public int TechnicianId { get; set; }
    }
}
