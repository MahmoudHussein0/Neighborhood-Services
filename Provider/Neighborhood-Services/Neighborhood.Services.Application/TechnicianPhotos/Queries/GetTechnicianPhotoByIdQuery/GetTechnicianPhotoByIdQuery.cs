using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.DTOs;

namespace Neighborhood.Services.Application.TechnicianPhotos.Queries
{
    public class GetTechnicianPhotoByIdQuery : IRequest<TechnicianPhotoDTO>
    {
        public int Id { get; set; }
    }
}
