using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Reviews.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Bookings.Queries.GetMyBookingsQuery
{
    public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, PagedResult<MyBookingSummaryDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyBookingsQueryHandler(
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            ITechnicianRepository technicianRepository,
            IReviewRepository reviewRepository,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _technicianRepository = technicianRepository;
            _reviewRepository = reviewRepository;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<MyBookingSummaryDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            // Normalize paging (guard against bad input)
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 100 ? 10 : request.PageSize;

            // Try customer first
            var customer = await _customerRepository.GetByUserIdAsync(userId);
            if (customer != null)
            {
                var paged = await _bookingRepository.GetCustomerBookingsPagedAsync(customer.Id, request.Status, request.Search, page, pageSize);
                return await MapToDtoAsync(paged, userId, cancellationToken);
            }

            // Fall back to technician
            var technician = await _technicianRepository.GetByUserIdAsync(userId);
            if (technician != null)
            {
                var paged = await _bookingRepository.GetTechnicianBookingsPagedAsync(technician.Id, request.Status, request.Search, page, pageSize);
                return await MapToDtoAsync(paged, userId, cancellationToken);
            }

            throw new ForbiddenException("User is not a customer or technician.");
        }

        private async Task<PagedResult<MyBookingSummaryDto>> MapToDtoAsync(
            PagedResult<Domain.Bookings.Booking> source, string userId, CancellationToken cancellationToken)
        {
            var ids = source.Items.Select(b => b.Id).ToList();
            var reviewed = await _reviewRepository.GetBookingIdsReviewedByAsync(ids, userId, cancellationToken);

            return new PagedResult<MyBookingSummaryDto>(
                source.Items.Select(b => new MyBookingSummaryDto
                {
                    Id = b.Id,
                    BookingType = b.BookingType,
                    Description = b.Description,
                    Address = b.Address,
                    ScheduledAt = b.ScheduledAt,
                    EstimatedPrice = b.EstimatedPrice,
                    FinalPrice = b.FinalPrice,
                    DurationMinutes = b.DurationMinutes,
                    Status = b.Status,
                    ClientConfirmed = b.ClientConfirmed,
                    CreatedAt = b.CreatedAt,
                    TechnicianId = b.TechnicianId,
                    ProblemTypeId = b.ProblemTypeId,
                    Latitude = b.Location?.Y ?? 0,
                    Longitude = b.Location?.X ?? 0,
                    HasReview = reviewed.Contains(b.Id)
                }).ToList(),
                source.TotalCount,
                source.Page,
                source.PageSize);
        }
    }
}
