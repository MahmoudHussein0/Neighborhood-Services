using MediatR;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class UpdateTechnicianVerificationStatusCommand : IRequest
    {
        public int Id { get; set; }
        public TechnicianVerificationStatus VerificationStatus { get; set; }
    }
}
