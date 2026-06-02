using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.DTOs;

namespace Neighborhood.Services.Application.TechnicianPhotos.Queries
{
    public class GetTechnicianPhotosByUserIdQuery : IRequest<List<TechnicianPhotoDTO>>
    {
        public string ApplicationUserId { get; set; } = string.Empty;
    }
}
