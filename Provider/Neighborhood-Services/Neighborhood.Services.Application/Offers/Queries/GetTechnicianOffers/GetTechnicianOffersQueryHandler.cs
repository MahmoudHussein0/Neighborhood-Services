using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Offers.DTOs;
using Neighborhood.Services.Application.Offers.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Offers.Queries.GetTechnicianOffers
{
    public class GetTechnicianOffersQueryHandler : IRequestHandler<GetTechnicianOffersQuery, PagedResult<OfferDto>>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetTechnicianOffersQueryHandler(
            IOfferRepository offerRepository,
            ITechnicianRepository technicianRepository,
            ICurrentUserService currentUserService)
        {
            _offerRepository = offerRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<OfferDto>> Handle(GetTechnicianOffersQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");
            var technician = await _technicianRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Technician", userId);

            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

            var paged = await _offerRepository.GetTechnicianOffersPagedAsync(technician.Id, request.Status, page, pageSize);

            return new PagedResult<OfferDto>(
                paged.Items.Select(o => new OfferDto
                {
                    Id = o.Id,
                    ServiceRequestId = o.ServiceRequestId,
                    TechnicianId = o.TechnicianId,
                    Price = o.Price,
                    EstimatedDuration = o.EstimatedDuration,
                    Message = o.Message,
                    ScheduledAt = o.ScheduledAt,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                }).ToList(),
                paged.TotalCount,
                paged.Page,
                paged.PageSize);
        }
    }
}
