using MediatR;
using Neighborhood.Services.Application.Bookings.Interface;
using Neighborhood.Services.Application.Bookings.Services;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.TechnitianAvailability;
using Neighborhood.Services.Domain.Bookings;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Application.Bookings.Commands.CreateBookingCommands
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, int>
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPriceEstimationService _priceEstimationService;
        private readonly ITechnicianAvailabilityRepository _technicianAvailabilityRepository;
        public CreateBookingCommandHandler(IBookingRepository bookingRepository, IUnitOfWork unitOfWork, IPriceEstimationService priceEstimationService, ITechnicianAvailabilityRepository technicianAvailabilityRepository)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _priceEstimationService = priceEstimationService;
            _technicianAvailabilityRepository = technicianAvailabilityRepository;
        }

        public async Task<int> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            var estimatedPrice = await _priceEstimationService.EstimateAsync(request.ProblemTypeId);
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

            // validating if he is already booked on that time
            var activeBooking = await _bookingRepository
                .GetActiveBookingForTechnicianAsync(request.TechnicianId, request.ScheduledAt);
            if (activeBooking != null)
                throw new ConflictException("Technician is not available at this time");

            var booking = new Booking
            {
                CustomerId = request.CustomerId,
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

            await _bookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return booking.Id;
        }
    }
}
