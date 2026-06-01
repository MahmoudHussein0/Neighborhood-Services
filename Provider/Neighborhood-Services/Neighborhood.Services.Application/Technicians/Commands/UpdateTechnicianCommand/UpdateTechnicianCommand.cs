using MediatR;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class UpdateTechnicianCommand : IRequest
    {
        public int Id { get; set; }
        public string NationalId { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public int MaxTravelDistance { get; set; }
    }
}
