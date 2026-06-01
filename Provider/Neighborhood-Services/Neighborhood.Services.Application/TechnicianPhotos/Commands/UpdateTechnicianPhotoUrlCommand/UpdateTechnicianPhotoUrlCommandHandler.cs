using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;

namespace Neighborhood.Services.Application.TechnicianPhotos.Commands
{
    public class UpdateTechnicianPhotoUrlCommandHandler(ITechnicianPhotoRepository technicianPhotoRepository) : IRequestHandler<UpdateTechnicianPhotoUrlCommand>
    {
        private readonly ITechnicianPhotoRepository _technicianPhotoRepository = technicianPhotoRepository;

        public async Task Handle(UpdateTechnicianPhotoUrlCommand request, CancellationToken cancellationToken)
        {
            var technicianPhoto = await _technicianPhotoRepository.GetByIdAsync(request.Id);
            if (technicianPhoto == null)
            {
                throw new KeyNotFoundException("Technician photo not found");
            }

            technicianPhoto.PhotoUrl = request.PhotoUrl;

            await _technicianPhotoRepository.UpdateAsync(technicianPhoto);
        }
    }
}
