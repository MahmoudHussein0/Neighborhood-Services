using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.DTOs;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
using Neighborhood.Services.Domain.TechnicianPhotos;

namespace Neighborhood.Services.Application.TechnicianPhotos.Queries
{
    public class GetTechnicianPhotoByIdHandler(ITechnicianPhotoRepository technicianPhotoRepository) : IRequestHandler<GetTechnicianPhotoByIdQuery, TechnicianPhotoDTO>
    {
        private readonly ITechnicianPhotoRepository _technicianPhotoRepository = technicianPhotoRepository;

        public async Task<TechnicianPhotoDTO> Handle(GetTechnicianPhotoByIdQuery request, CancellationToken cancellationToken)
        {
            var technicianPhoto = await _technicianPhotoRepository.GetByIdAsync(request.Id);
            if (technicianPhoto == null)
            {
                throw new KeyNotFoundException("Technician photo not found");
            }

            return MapToDto(technicianPhoto);
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
