using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.DTOs;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
using Neighborhood.Services.Domain.TechnicianPhotos;

namespace Neighborhood.Services.Application.TechnicianPhotos.Queries
{
    public class GetAllTechnicianPhotosHandler(ITechnicianPhotoRepository technicianPhotoRepository) : IRequestHandler<GetAllTechnicianPhotosQuery, List<TechnicianPhotoDTO>>
    {
        private readonly ITechnicianPhotoRepository _technicianPhotoRepository = technicianPhotoRepository;

        public async Task<List<TechnicianPhotoDTO>> Handle(GetAllTechnicianPhotosQuery request, CancellationToken cancellationToken)
        {
            var technicianPhotos = await _technicianPhotoRepository.GetAllPhotosAsync();

            return technicianPhotos.Select(MapToDto).ToList();
        }

        private static TechnicianPhotoDTO MapToDto(TechnicianPhoto technicianPhoto)
        {
            return new TechnicianPhotoDTO
            {
                Id = technicianPhoto.Id,
                PhotoUrl = technicianPhoto.PhotoUrl,
                Caption = technicianPhoto.Caption,
                CreatedAt = technicianPhoto.CreatedAt,
                ApplicationUserId = technicianPhoto.ApplicationUserId,
                TechnicianId = technicianPhoto.TechnicianId
            };
        }
    }
}
