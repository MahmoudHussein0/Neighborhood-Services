using MediatR;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class DeleteTechnicianCommandHandler(ITechnicianRepository technicianRepository) : IRequestHandler<DeleteTechnicianCommand>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task Handle(DeleteTechnicianCommand request, CancellationToken cancellationToken)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.Id);
            if (technician == null)
            {
                throw new KeyNotFoundException("Technician not found");
            }

            technician.IsDeleted = true;
            technician.IsActive = false;
            technician.IsAvailable = false;
            technician.UpdatedAt = DateTime.UtcNow;

            await _technicianRepository.UpdateAsync(technician);
        }
    }
}
