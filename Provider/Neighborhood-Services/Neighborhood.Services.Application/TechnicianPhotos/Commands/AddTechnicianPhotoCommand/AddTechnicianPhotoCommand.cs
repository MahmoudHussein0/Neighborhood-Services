using MediatR;

namespace Neighborhood.Services.Application.TechnicianPhotos.Commands
{
    public class AddTechnicianPhotoCommand : IRequest<int>
    {
        public string PhotoUrl { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;
        public int TechnicianId { get; set; }
    }
}
