using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;

namespace Neighborhood.Services.Application.TechnicianPhotos.Commands
{
    public class UpdateTechnicianPhotoCaptionCommandHandler(ITechnicianPhotoRepository technicianPhotoRepository) : IRequestHandler<UpdateTechnicianPhotoCaptionCommand>
    {
        private readonly ITechnicianPhotoRepository _technicianPhotoRepository = technicianPhotoRepository;

        public async Task Handle(UpdateTechnicianPhotoCaptionCommand request, CancellationToken cancellationToken)
        {
            var technicianPhoto = await _technicianPhotoRepository.GetByIdAsync(request.Id);
            if (technicianPhoto == null)
            {
                throw new KeyNotFoundException("Technician photo not found");
            }

            technicianPhoto.Caption = request.Caption;

            await _technicianPhotoRepository.UpdateAsync(technicianPhoto);
        }
    }
}
