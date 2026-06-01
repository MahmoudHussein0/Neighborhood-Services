using MediatR;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class ActivateTechnicianCommand : IRequest
    {
        public int Id { get; set; }
    }
}
