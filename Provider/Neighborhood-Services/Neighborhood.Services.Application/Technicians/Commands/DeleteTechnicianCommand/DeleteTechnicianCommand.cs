using MediatR;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class DeleteTechnicianCommand : IRequest
    {
        public int Id { get; set; }
    }
}
