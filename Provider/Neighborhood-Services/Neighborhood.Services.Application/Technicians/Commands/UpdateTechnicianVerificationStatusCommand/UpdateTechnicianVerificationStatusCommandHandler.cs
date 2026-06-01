using MediatR;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Technicians.Commands
{
    public class UpdateTechnicianVerificationStatusCommandHandler(ITechnicianRepository technicianRepository) : IRequestHandler<UpdateTechnicianVerificationStatusCommand>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task Handle(UpdateTechnicianVerificationStatusCommand request, CancellationToken cancellationToken)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.Id);
            if (technician == null)
            {
                throw new KeyNotFoundException("Technician not found");
            }

            technician.VerificationStatus = request.VerificationStatus;
            technician.UpdatedAt = DateTime.UtcNow;

            await _technicianRepository.UpdateAsync(technician);
        }
    }
}
