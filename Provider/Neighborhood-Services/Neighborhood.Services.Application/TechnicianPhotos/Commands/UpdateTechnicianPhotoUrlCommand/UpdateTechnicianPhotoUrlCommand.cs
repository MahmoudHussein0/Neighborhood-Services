using MediatR;

namespace Neighborhood.Services.Application.TechnicianPhotos.Commands
{
    public class UpdateTechnicianPhotoUrlCommand : IRequest
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
