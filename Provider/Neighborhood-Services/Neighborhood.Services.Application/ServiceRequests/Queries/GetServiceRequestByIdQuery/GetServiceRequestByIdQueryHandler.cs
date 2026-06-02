using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ServiceRequests.DTOs;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.ServiceRequests.Queries.GetServiceRequestByIdQuery
{
    public class GetServiceRequestByIdQueryHandler : IRequestHandler<GetServiceRequestByIdQuery, ServiceRequestDetailsDto>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public GetServiceRequestByIdQueryHandler(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<ServiceRequestDetailsDto> Handle(GetServiceRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var sr = await _serviceRequestRepository.GetServiceRequestWithDetailsAsync(request.ServiceRequestId);

            if (sr is null)
                throw new NotFoundException(nameof(ServiceRequest), request.ServiceRequestId);

            return new ServiceRequestDetailsDto
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
                OfferCount=sr.Offers?.Count?? 0
            };
        }
    }
}
