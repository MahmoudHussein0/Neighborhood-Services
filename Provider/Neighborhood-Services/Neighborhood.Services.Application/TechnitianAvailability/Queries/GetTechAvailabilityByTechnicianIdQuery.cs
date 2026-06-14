using MediatR;
using Neighborhood.Services.Application.TechnitianAvailability.DTOs;

namespace Neighborhood.Services.Application.TechnitianAvailability.Queries
{
    /// <summary>
    /// Fetch a specific technician's working days + hours by their technician id.
    /// Used by customers (e.g. the create-booking modal) — does not rely on the current user.
    /// </summary>
    public class GetTechAvailabilityByTechnicianIdQuery : IRequest<IReadOnlyList<TechnicianAvailabilityDetailsDTO>>
    {
        public int TechnicianId { get; set; }
    }
}
