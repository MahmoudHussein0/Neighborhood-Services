using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;

namespace Neighborhood.Services.Application.TechnicianPhotos.Commands
{
    public class DeleteTechnicianPhotoCommandHandler(ITechnicianPhotoRepository technicianPhotoRepository) : IRequestHandler<DeleteTechnicianPhotoCommand>
    {
        private readonly ITechnicianPhotoRepository _technicianPhotoRepository = technicianPhotoRepository;

        public async Task Handle(DeleteTechnicianPhotoCommand request, CancellationToken cancellationToken)
        {
            var technicianPhoto = await _technicianPhotoRepository.GetByIdAsync(request.Id);
            if (technicianPhoto == null)
            {
                throw new KeyNotFoundException("Technician photo not found");
            }

            await _technicianPhotoRepository.DeletePhotoAsync(technicianPhoto);
        }
    }
}
