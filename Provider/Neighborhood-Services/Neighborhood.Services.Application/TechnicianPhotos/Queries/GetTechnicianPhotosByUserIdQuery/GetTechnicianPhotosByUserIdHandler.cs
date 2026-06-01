using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.DTOs;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
using Neighborhood.Services.Domain.TechnicianPhotos;

namespace Neighborhood.Services.Application.TechnicianPhotos.Queries
{
    public class GetTechnicianPhotosByUserIdHandler(ITechnicianPhotoRepository technicianPhotoRepository) : IRequestHandler<GetTechnicianPhotosByUserIdQuery, List<TechnicianPhotoDTO>>
    {
        private readonly ITechnicianPhotoRepository _technicianPhotoRepository = technicianPhotoRepository;

        public async Task<List<TechnicianPhotoDTO>> Handle(GetTechnicianPhotosByUserIdQuery request, CancellationToken cancellationToken)
        {
            var technicianPhotos = await _technicianPhotoRepository.GetByUserIdAsync(request.ApplicationUserId);

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
