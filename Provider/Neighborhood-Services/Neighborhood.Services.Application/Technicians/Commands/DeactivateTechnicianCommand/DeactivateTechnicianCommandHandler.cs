using MediatR;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class DeactivateTechnicianCommandHandler(ITechnicianRepository technicianRepository) : IRequestHandler<DeactivateTechnicianCommand>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task Handle(DeactivateTechnicianCommand request, CancellationToken cancellationToken)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.Id);
            if (technician == null)
            {
                throw new KeyNotFoundException("Technician not found");
            }

            technician.IsActive = false;
            technician.UpdatedAt = DateTime.UtcNow;

            await _technicianRepository.UpdateAsync(technician);
        }
    }
}
