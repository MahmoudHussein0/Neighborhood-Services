using MediatR;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class UpdateTechnicianAvailabilityCommandHandler(ITechnicianRepository technicianRepository) : IRequestHandler<UpdateTechnicianAvailabilityCommand>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task Handle(UpdateTechnicianAvailabilityCommand request, CancellationToken cancellationToken)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.Id);
            if (technician == null)
            {
                throw new KeyNotFoundException("Technician not found");
            }

            technician.IsAvailable = request.IsAvailable;
            technician.UpdatedAt = DateTime.UtcNow;

            await _technicianRepository.UpdateAsync(technician);
        }
    }
}
