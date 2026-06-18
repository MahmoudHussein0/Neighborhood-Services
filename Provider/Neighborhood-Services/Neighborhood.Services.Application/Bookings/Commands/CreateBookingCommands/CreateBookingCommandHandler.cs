using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Bookings.Services;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.PromoCodes.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Application.TechnitianAvailability.Interfaces;
using Neighborhood.Services.Domain.Bookings;
using Neighborhood.Services.Domain.PromoCodes;
using NetTopologySuite.Geometries;
using System.Collections.Concurrent;

namespace Neighborhood.Services.Application.Bookings.Commands.CreateBookingCommands
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, int>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPriceEstimationService _priceEstimationService;
        private readonly ITechnicianAvailabilityRepository _technicianAvailabilityRepository;
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<CreateBookingCommandHandler> _logger;
        // trying to handle concurrency
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
        public CreateBookingCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork, IPriceEstimationService priceEstimationService, ITechnicianAvailabilityRepository technicianAvailabilityRepository, IPromoCodeRepository promoCodeRepository, ICurrentUserService currentUserService, ICustomerRepository customerRepository, ITechnicianRepository technicianRepository, INotificationService notificationService, ILogger<CreateBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _priceEstimationService = priceEstimationService;
            _technicianAvailabilityRepository = technicianAvailabilityRepository;
            _promoCodeRepository = promoCodeRepository;
            _currentUserService = currentUserService;
            _customerRepository = customerRepository;
            _technicianRepository = technicianRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            PromoCode? promoCode = null;

            // Resolve the customer from the authenticated user
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");
            var customer = await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Customer", userId);

            // Trying the Lock : Per technician + time slot lock
            var lockKey = $"{request.TechnicianId}_{request.ScheduledAt:yyyyMMddHHmm}";
            var semaphore = _locks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var estimatedPrice = await _priceEstimationService.EstimateAsync(request.ProblemTypeId , request.Region );
            // validating the date 
            if (request.ScheduledAt <= DateTime.UtcNow)
                throw new ValidationException("Scheduled time cannot be in the past");
            // Validating if he works at that day at all ??
            // Does the technician work on this day at all?
            var availability = await _technicianAvailabilityRepository
                .GetByConditionAsync(a => a.TechnicianId == request.TechnicianId);

            var dayAvailability = availability
                .FirstOrDefault(a => a.DayOfWeek == request.ScheduledAt.DayOfWeek);

            if (dayAvailability == null)
                throw new ValidationException("Technician does not work on this day");

            if (request.ScheduledAt.TimeOfDay < dayAvailability.StartTime.ToTimeSpan() ||
                request.ScheduledAt.TimeOfDay > dayAvailability.EndTime.ToTimeSpan())
                throw new ValidationException("Scheduled time is outside technician working hours");

            // Reject up-front if the requested start lands inside one of the tech's existing
            // confirmed bookings. The new booking has no duration yet (the tech sets it when
            // quoting), so we can only check the start instant here — we probe a 1-minute window
            // so a start exactly on/inside an existing job is caught. The precise duration-aware
            // overlap check still runs at quote time as the final guard.
            var startsDuringAnotherJob = await _bookingRepository.HasOverlappingConfirmedBookingAsync(
                request.TechnicianId,
                request.ScheduledAt,
                request.ScheduledAt.AddMinutes(1));
            if (startsDuringAnotherJob)
                throw new ConflictException("Sorry, the technician is busy at this time. Please pick another slot.");
            if (request.PromoCodeId.HasValue)
            {
                 promoCode = await _promoCodeRepository.GetByIdAsync(request.PromoCodeId.Value);

                if (promoCode is null)
                    throw new NotFoundException("PromoCode", request.PromoCodeId.Value);

                if (!promoCode.IsActive)
                    throw new BadRequestException("Promo code is not active");

                if (promoCode.ExpiresAt < DateTime.UtcNow)
                    throw new BadRequestException("Promo code has expired");

                if (promoCode.UsedCount >= promoCode.MaxUses)
                    throw new BadRequestException("Promo code has reached maximum uses");
            }

            var booking = new Booking
            {
                CustomerId = customer.Id,
                ProblemTypeId = request.ProblemTypeId,
                Description = request.Description,
                Address = request.Address,
                ScheduledAt = request.ScheduledAt,
                BookingType = BookingType.Direct,
                TechnicianId = request.TechnicianId,
                PromoCodeId = request.PromoCodeId,
                Status = BookingStatus.Pending,
                EstimatedPrice = estimatedPrice,
                FinalPrice = 0,
                Location = new Point(request.Longitude, request.Latitude) { SRID = 4326 },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Attach the customer's "Before" photo (if provided) so it's inserted atomically
            // with the booking — the technician sees it when quoting.
            if (!string.IsNullOrWhiteSpace(request.BeforeImageUrl))
            {
                booking.BookingImages.Add(new Domain.BookingImages.BookingImage
                {
                    ImageUrl = request.BeforeImageUrl,
                    Type = Domain.BookingImages.BookingImageType.Before,
                    UploadedBy = userId,
                    UploadedAt = DateTime.UtcNow
                });
            }

            await _bookingRepository.AddAsync(booking);
            // Increment promo code usage
            if (request.PromoCodeId.HasValue)
            {
                promoCode.UsedCount++;
                await _promoCodeRepository.UpdateAsync(promoCode);
            }
                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateException)
                {
                    throw new ConflictException("Technician is not available at this time");
                }

                // Notify the technician of the new booking request — best effort.
                try
                {
                    var technician = await _technicianRepository.GetByIdAsync(request.TechnicianId);
                    if (!string.IsNullOrEmpty(technician?.ApplicationUserId))
                        await _notificationService.SendNotificationToUser(
                            technician.ApplicationUserId,
                            $"You have a new booking request (#{booking.Id}). Review it and send a quote.");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "New-booking notification failed for booking {Id}.", booking.Id);
                }

                return booking.Id;
            }
            finally
            {
                semaphore.Release();
                // Clean up the lock entry if no other thread is waiting on it
                // CurrentCount == 1 means the semaphore is back to fully available
                if (semaphore.CurrentCount == 1)
                    _locks.TryRemove(lockKey, out _);
            }
        }
    }
}
