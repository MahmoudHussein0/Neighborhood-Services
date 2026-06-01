using MediatR;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class UpdateTechnicianAvailabilityCommand : IRequest
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; }
    }
}
