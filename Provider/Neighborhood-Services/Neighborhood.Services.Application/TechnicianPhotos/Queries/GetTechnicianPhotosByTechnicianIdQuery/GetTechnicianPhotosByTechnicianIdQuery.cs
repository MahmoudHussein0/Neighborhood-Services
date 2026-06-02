using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.DTOs;

namespace Neighborhood.Services.Application.TechnicianPhotos.Queries
{
    public class GetTechnicianPhotosByTechnicianIdQuery : IRequest<List<TechnicianPhotoDTO>>
    {
        public int TechnicianId { get; set; }
    }
}
