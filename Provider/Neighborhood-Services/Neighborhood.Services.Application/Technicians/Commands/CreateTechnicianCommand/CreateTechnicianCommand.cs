using MediatR;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class CreateTechnicianCommand : IRequest<int>
    {
        public string ApplicationUserId { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public int MaxTravelDistance { get; set; }
    }
}
