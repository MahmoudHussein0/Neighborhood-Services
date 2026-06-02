using MediatR;
using Neighborhood.Services.Application.Technicians.DTOs;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetTechnicianByIdHandler(ITechnicianRepository technicianRepository) : IRequestHandler<GetTechnicianByIdQuery, TechnicianDetailsDTO>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task<TechnicianDetailsDTO> Handle(GetTechnicianByIdQuery request, CancellationToken cancellationToken)
        {
            var technician = await _technicianRepository.GetByIdAsync(request.Id);
            if (technician == null)
            {
                throw new KeyNotFoundException("Technician not found");
            }

            return MapToDetails(technician);
        }

        private static TechnicianDetailsDTO MapToDetails(Technician technician)
        {
            return new TechnicianDetailsDTO
            {
                Id = technician.Id,
                ApplicationUserId = technician.ApplicationUserId,
                NationalId = technician.NationalId,
                Experience = technician.Experience,
                Rating = technician.Rating,
                MaxTravelDistance = technician.MaxTravelDistance,
                VerificationStatus = technician.VerificationStatus,
                IsAvailable = technician.IsAvailable,
                IsDeleted = technician.IsDeleted,
                IsActive = technician.IsActive,
                CreatedAt = technician.CreatedAt,
                UpdatedAt = technician.UpdatedAt
            };
        }
    }
}
