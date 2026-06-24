using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.RecurringBookings.DTOs;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Application.RecurringBookings.Queries.GetRecurringBookingByIdQuery;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.RecurringBookings.Queries.GetMyRecurringBookingsQuery
{
    public class GetMyRecurringBookingsQueryHandler : IRequestHandler<GetMyRecurringBookingsQuery, PagedResult<RecurringBookingDto>>
    {
        private readonly IRecurringBookingRepository _repository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyRecurringBookingsQueryHandler(
            IRecurringBookingRepository repository,
            ICustomerRepository customerRepository,
            ITechnicianRepository technicianRepository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _customerRepository = customerRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<RecurringBookingDto>> Handle(GetMyRecurringBookingsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            // Normalize paging (guard against bad input)
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

            var customer = await _customerRepository.GetByUserIdAsync(userId);
            if (customer != null)
            {
                var paged = await _repository.GetCustomerRecurringBookingsPagedAsync(customer.Id, request.Status, request.Search, page, pageSize);
                return await MapToDtoAsync(paged);
            }

            var technician = await _technicianRepository.GetByUserIdAsync(userId);
            if (technician != null)
            {
                var paged = await _repository.GetTechnicianRecurringBookingsPagedAsync(technician.Id, request.Status, request.Search, page, pageSize);
                return await MapToDtoAsync(paged);
            }

            throw new ForbiddenException("User is not a customer or technician.");
        }

        // Maps entities -> DTOs and fills in both parties' names so the details view shows real
        // names (technician for the customer, customer for the technician) instead of raw ids.
        private async Task<PagedResult<RecurringBookingDto>> MapToDtoAsync(PagedResult<Domain.RecurringBookings.RecurringBooking> source)
        {
            var technicianNames = await _technicianRepository.GetNamesByIdsAsync(
                source.Items.Select(r => r.TechnicianId).Distinct().ToList());
            var customerNames = await _customerRepository.GetNamesByIdsAsync(
                source.Items.Select(r => r.CustomerId).Distinct().ToList());

            var items = source.Items.Select(rb =>
            {
                var dto = GetRecurringBookingByIdQueryHandler.MapToDto(rb);
                dto.TechnicianName = technicianNames.GetValueOrDefault(rb.TechnicianId, string.Empty);
                dto.CustomerName = customerNames.GetValueOrDefault(rb.CustomerId, string.Empty);
                return dto;
            }).ToList();

            return new PagedResult<RecurringBookingDto>(items, source.TotalCount, source.Page, source.PageSize);
        }
    }
}
