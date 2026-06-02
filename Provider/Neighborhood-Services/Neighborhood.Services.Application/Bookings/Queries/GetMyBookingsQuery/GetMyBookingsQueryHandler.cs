using MediatR;
using Neighborhood.Services.Application.Bookings.DTOs;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;

namespace Neighborhood.Services.Application.Bookings.Queries.GetMyBookingsQuery
{
    public class GetMyBookingsQueryHandler : IRequestHandler<GetMyBookingsQuery, IEnumerable<BookingSummaryDto>>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyBookingsQueryHandler(
            IBookingRepository bookingRepository,
            ICustomerRepository customerRepository,
            ITechnicianRepository technicianRepository,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _customerRepository = customerRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<BookingSummaryDto>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            // Try customer first
            var customer = await _customerRepository.GetByUserIdAsync(userId);
            if (customer != null)
            {
                var bookings = await _bookingRepository.GetCustomerBookingsAsync(customer.Id);
                return MapToDto(bookings);
            }

            // Fall back to technician
            var technician = await _technicianRepository.GetByUserIdAsync(userId);
            if (technician != null)
            {
                var bookings = await _bookingRepository.GetTechnicianBookingsAsync(technician.Id);
                return MapToDto(bookings);
            }

            throw new ForbiddenException("User is not a customer or technician.");
        }

        private static IEnumerable<BookingSummaryDto> MapToDto(IEnumerable<Domain.Bookings.Booking> bookings) =>
            bookings.Select(b => new BookingSummaryDto
            {
                Id = b.Id,
                BookingType = b.BookingType,
                Description = b.Description,
                Address = b.Address,
                ScheduledAt = b.ScheduledAt,
                EstimatedPrice = b.EstimatedPrice,
                Status = b.Status,
                CreatedAt = b.CreatedAt
            });
    }
}
