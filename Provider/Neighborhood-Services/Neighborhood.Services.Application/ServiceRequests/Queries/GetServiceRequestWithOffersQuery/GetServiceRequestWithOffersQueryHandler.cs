using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestWithOffersQuery
{
    public class GetServiceRequestWithOffersQueryHandler : IRequestHandler<GetServiceRequestWithOffersQuery, ServiceRequestWithOffersDto>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public GetServiceRequestWithOffersQueryHandler(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<ServiceRequestWithOffersDto> Handle(GetServiceRequestWithOffersQuery request, CancellationToken cancellationToken)
        {
            var sr = await _serviceRequestRepository.GetServiceRequestWithOffersAsync(request.ServiceRequestId);

            if (sr is null)
                throw new NotFoundException(nameof(ServiceRequest), request.ServiceRequestId);

            // Offers only carry TechnicianId; resolve each technician's name for the offer cards.
            var technicianIds = sr.Offers?.Select(o => o.TechnicianId).Distinct().ToList()
                ?? new List<int>();
            var technicianNames = await _serviceRequestRepository.GetTechnicianNamesAsync(technicianIds);

            return new ServiceRequestWithOffersDto
            {
                Id = sr.Id,
                Description = sr.Description,
                Address = sr.Address,
                Image = sr.Image,
                Budget = sr.Budget,
                Status = sr.Status,
                ScheduledAt = sr.ScheduledAt,
                CategoryId = sr.CategoryId,
                ProblemTypeId = sr.ProblemTypeId,
                CustomerId = sr.CustomerId,
                Latitude = sr.Location.Y,
                Longitude = sr.Location.X,
                CreatedAt = sr.CreatedAt,
                ExpiresAt = sr.ExpiresAt,
                OfferCount = sr.Offers?.Count ?? 0,
                Offers = sr.Offers?.Select(o => new OfferSummaryDto
                {
                    Id = o.Id,
                    Price = o.Price,
                    EstimatedDuration = o.EstimatedDuration,
                    Message = o.Message,
                    Status = o.Status,
                    TechnicianId = o.TechnicianId,
                    TechnicianName = technicianNames.GetValueOrDefault(o.TechnicianId, string.Empty),
                    TechnicianRating = o.Technician != null ? (double)o.Technician.Rating : 0,
                    CreatedAt = o.CreatedAt
                }).ToList() ?? new List<OfferSummaryDto>()
            };
        
        }
    }
}
