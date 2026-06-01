using MediatR;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class DeactivateTechnicianCommand : IRequest
    {
        public int Id { get; set; }
    }
}
