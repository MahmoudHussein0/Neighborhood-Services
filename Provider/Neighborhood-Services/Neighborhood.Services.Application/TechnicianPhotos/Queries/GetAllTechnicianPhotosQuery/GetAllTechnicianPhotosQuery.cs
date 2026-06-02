using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.DTOs;

namespace Neighborhood.Services.Application.TechnicianPhotos.Queries
{
    public class GetAllTechnicianPhotosQuery : IRequest<List<TechnicianPhotoDTO>>
    {
    }
}
