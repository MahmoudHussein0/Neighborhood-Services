using MediatR;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class UpdateTechnicianCommandHandler(ITechnicianRepository technicianRepository) : IRequestHandler<UpdateTechnicianCommand>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task Handle(UpdateTechnicianCommand request, CancellationToken cancellationToken)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.Id);
            if (technician == null)
            {
                throw new KeyNotFoundException("Technician not found");
            }

            technician.NationalId = request.NationalId;
            technician.Experience = request.Experience;
            technician.MaxTravelDistance = request.MaxTravelDistance;
            technician.UpdatedAt = DateTime.UtcNow;

            await _technicianRepository.UpdateAsync(technician);
        }
    }
}
