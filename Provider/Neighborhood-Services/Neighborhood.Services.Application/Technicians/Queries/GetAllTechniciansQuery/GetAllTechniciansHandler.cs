using MediatR;
using Neighborhood.Services.Application.Technicians.DTOs;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetAllTechniciansHandler(ITechnicianRepository technicianRepository) : IRequestHandler<GetAllTechniciansQuery, List<TechnicianSummaryDTO>>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task<List<TechnicianSummaryDTO>> Handle(GetAllTechniciansQuery request, CancellationToken cancellationToken)
        {
            var technicians = await _technicianRepository.GetAllActiveAsync();

            return technicians.Select(MapToSummary).ToList();
        }

        private static TechnicianSummaryDTO MapToSummary(Technician technician)
        {
            return new TechnicianSummaryDTO
            {
                Id = technician.Id,
                ApplicationUserId = technician.ApplicationUserId,
                NationalId = technician.NationalId,
                Rating = technician.Rating,
                MaxTravelDistance = technician.MaxTravelDistance,
                VerificationStatus = technician.VerificationStatus,
                IsAvailable = technician.IsAvailable,
                IsActive = technician.IsActive
            };
        }
    }
}
