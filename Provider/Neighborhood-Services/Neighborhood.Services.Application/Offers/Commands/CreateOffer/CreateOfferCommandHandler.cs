using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.Offers.DTOs;
using Neighborhood.Services.Application.Offers.Interfaces;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;
using Neighborhood.Services.Domain.Offers;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.Offers.Commands.CreateOffer
{
    public class CreateOfferCommandHandler : IRequestHandler<CreateOfferCommand, CreateOfferResultDto>
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ITechnicianAvailabilityRepository _availabilityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<CreateOfferCommandHandler> _logger;

        public CreateOfferCommandHandler(
            IOfferRepository offerRepository,
            IServiceRequestRepository serviceRequestRepository,
            IBookingRepository bookingRepository,
            ITechnicianRepository technicianRepository,
            ITechnicianAvailabilityRepository availabilityRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ICustomerRepository customerRepository,
            INotificationService notificationService,
            ILogger<CreateOfferCommandHandler> logger)
        {
            _offerRepository = offerRepository;
            _serviceRequestRepository = serviceRequestRepository;
            _bookingRepository = bookingRepository;
            _technicianRepository = technicianRepository;
            _availabilityRepository = availabilityRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<CreateOfferResultDto> Handle(CreateOfferCommand request, CancellationToken cancellationToken)
        {
            if (request.Price <= 0)
                throw new ValidationException("Offer price must be greater than zero.");

            if (request.EstimatedDuration <= 0)
                throw new ValidationException("Estimated duration must be greater than zero.");

            // Proposed time must be in the future (hard block)
            if (request.ScheduledAt <= DateTime.UtcNow)
                throw new ValidationException("Proposed time cannot be in the past.");

            // Resolve the technician from the authenticated user
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");
            var technician = await _technicianRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Technician", userId);

            // Service request must exist and be open
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(request.ServiceRequestId);
            if (serviceRequest is null)
                throw new NotFoundException(nameof(ServiceRequest), request.ServiceRequestId);

            if (serviceRequest.Status != ServiceRequestStatus.Open)
                throw new BadRequestException("This service request is no longer open for offers.");

            // A technician may only offer on requests within their assigned service categories.
            var technicianCategoryIds = await _technicianRepository.GetCategoryIdsAsync(technician.Id);
            if (!technicianCategoryIds.Contains(serviceRequest.CategoryId))
                throw new ForbiddenException("You can only make offers on requests within your service categories.");

            // One pending offer per technician per service request
            var existingOffers = await _offerRepository.GetOffersByServiceRequestAsync(request.ServiceRequestId);
            if (existingOffers.Any(o => o.TechnicianId == technician.Id && o.Status == OfferStatus.Pending))
                throw new ConflictException("You already have a pending offer on this service request.");

            // Hard block: technician must not have a confirmed booking overlapping the proposed time
            var start = request.ScheduledAt;
            var end = start.AddMinutes(request.EstimatedDuration);
            var hasOverlap = await _bookingRepository.HasOverlappingConfirmedBookingAsync(technician.Id, start, end);
            if (hasOverlap)
                throw new ConflictException("You already have a confirmed booking that overlaps the proposed time.");

            // Soft warning: proposed time is outside the technician's usual working hours
            var warnings = new List<string>();
            var availability = await _availabilityRepository
                .GetByConditionAsync(a => a.TechnicianId == technician.Id);
            var dayAvailability = availability.FirstOrDefault(a => a.DayOfWeek == request.ScheduledAt.DayOfWeek);

            if (dayAvailability is null)
            {
                warnings.Add("Heads up: you don't usually work on this day.");
            }
            else if (request.ScheduledAt.TimeOfDay < dayAvailability.StartTime.ToTimeSpan() ||
                     request.ScheduledAt.TimeOfDay > dayAvailability.EndTime.ToTimeSpan())
            {
                warnings.Add("Heads up: the proposed time is outside your usual working hours.");
            }

            var offer = new Offer
            {
                ServiceRequestId = request.ServiceRequestId,
                TechnicianId = technician.Id,
                Price = request.Price,
                EstimatedDuration = request.EstimatedDuration,
                Message = request.Message,
                ScheduledAt = request.ScheduledAt,
                Status = OfferStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _offerRepository.AddAsync(offer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the request's customer that a new offer arrived — best effort.
            try
            {
                var customer = await _customerRepository.GetByIdAsync(serviceRequest.CustomerId);
                if (!string.IsNullOrEmpty(customer?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        customer.ApplicationUserId,
                        $"You received a new offer on your service request #{serviceRequest.Id}.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "New-offer notification failed for request {Id}.", serviceRequest.Id);
            }

            return new CreateOfferResultDto
            {
                OfferId = offer.Id,
                Warnings = warnings
            };
        }
    }
}
