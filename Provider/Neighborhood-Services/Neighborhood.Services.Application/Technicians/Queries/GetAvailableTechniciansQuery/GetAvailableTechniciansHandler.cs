using MediatR;
using Neighborhood.Services.Application.Technicians.DTOs;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.Technicians;

namespace Neighborhood.Services.Application.Technicians.Queries
{
    public class GetAvailableTechniciansHandler(ITechnicianRepository technicianRepository) : IRequestHandler<GetAvailableTechniciansQuery, List<TechnicianSummaryDTO>>
    {
        private readonly ITechnicianRepository _technicianRepository = technicianRepository;

        public async Task<List<TechnicianSummaryDTO>> Handle(GetAvailableTechniciansQuery request, CancellationToken cancellationToken)
        {
            var technicians = await _technicianRepository.GetAvailableAsync();

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
