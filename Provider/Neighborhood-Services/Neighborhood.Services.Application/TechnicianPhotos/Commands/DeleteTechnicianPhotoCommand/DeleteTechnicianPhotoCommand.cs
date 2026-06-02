using MediatR;

namespace Neighborhood.Services.Application.TechnicianPhotos.Commands
{
    public class DeleteTechnicianPhotoCommand : IRequest
    {
        public int Id { get; set; }
    }
}
