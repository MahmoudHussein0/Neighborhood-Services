using MediatR;

namespace Neighborhood.Services.Application.TechnicianPhotos.Commands
{
    public class UpdateTechnicianPhotoCaptionCommand : IRequest
    {
        public int Id { get; set; }
        public string Caption { get; set; } = string.Empty;
    }
}
