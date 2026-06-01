using MediatR;
using Neighborhood.Services.Application.TechnicianPhotos.Interfaces;
using Neighborhood.Services.Domain.TechnicianPhotos;

namespace Neighborhood.Services.Application.TechnicianPhotos.Commands
{
    public class AddTechnicianPhotoCommandHandler(ITechnicianPhotoRepository technicianPhotoRepository) : IRequestHandler<AddTechnicianPhotoCommand, int>
    {
        private readonly ITechnicianPhotoRepository _technicianPhotoRepository = technicianPhotoRepository;

        public async Task<int> Handle(AddTechnicianPhotoCommand request, CancellationToken cancellationToken)
        {
            var technicianPhoto = new TechnicianPhoto
            {
                PhotoUrl = request.PhotoUrl,
                Caption = request.Caption,
                ApplicationUserId = request.ApplicationUserId,
                TechnicianId = request.TechnicianId,
                CreatedAt = DateTime.UtcNow
            };

            await _technicianPhotoRepository.CreateAsync(technicianPhoto);
            return technicianPhoto.Id;
        }
    }
}
