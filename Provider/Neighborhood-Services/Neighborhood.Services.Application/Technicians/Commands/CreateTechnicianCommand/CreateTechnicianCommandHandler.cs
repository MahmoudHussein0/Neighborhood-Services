using MediatR;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class CreateTechnicianCommandHandler(ITechnicianRepository technicianRepository) : IRequestHandler<CreateTechnicianCommand, int>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task<int> Handle(CreateTechnicianCommand request, CancellationToken cancellationToken)
        {
            var technician = new Technician
            {
                ApplicationUserId = request.ApplicationUserId,
                NationalId = request.NationalId,
                Experience = request.Experience,
                MaxTravelDistance = request.MaxTravelDistance,
                Rating = 0,
                VerificationStatus = TechnicianVerificationStatus.Pending,
                IsAvailable = true,
                IsDeleted = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _technicianRepository.CreateAsync(technician);
            return technician.Id;
        }
    }
}
