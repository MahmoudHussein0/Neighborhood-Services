using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.RecurringBookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.CreateRecurring
{
    public class CreateRecurringBookingCommandHandler : IRequestHandler<CreateRecurringBookingCommand,int>
    {
        private readonly IRecurringBookingRepository _recurringBookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICustomerRepository _customerRepository;
        public CreateRecurringBookingCommandHandler(IUnitOfWork unitOfWork, IRecurringBookingRepository recurringBookingRepository, ICurrentUserService currentUserService, ITechnicianRepository technicianRepository, ICustomerRepository customerRepository)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _technicianRepository = technicianRepository;
            _customerRepository = customerRepository;
        }
        public async Task<int> Handle(CreateRecurringBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId ?? throw new UnauthorizedException("User is Not authenticated ");

            // validate start to be in future 
            if (request.StartDate <= DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ValidationException("start date must be in the future ");
            // Validate end date after start date 
            if (request.EndDate.HasValue && request.EndDate <= request.StartDate)
                throw new ValidationException("End date must be after start date.");
            // Validate pattern fields
            if (request.Pattern == RecurringPattern.Weekly && request.DayOfWeek == null)
                throw new ValidationException("Day of week is required for weekly pattern.");

            if (request.Pattern == RecurringPattern.Monthly && request.DayOfMonth == null)
                throw new ValidationException("Day of month is required for monthly pattern.");

            if (request.DurationMinutes <= 0)
                throw new ValidationException("Duration must be greater than zero.");
            // validate if the tech exists && is active ?
             var technician = await _technicianRepository.GetByIdAsync(request.TechnicianId)
                 ?? throw new NotFoundException("Technician", request.TechnicianId);

            if (!technician.IsActive)
                throw new BadRequestException("This technician is not currently active.");

            // getting the customer from the current user 
            var customer = await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Customer", userId);

            var recurringBooking = new RecurringBooking
            {
                CustomerId = customer.Id,
                TechnicianId = request.TechnicianId,
                ProblemTypeId = request.ProblemTypeId,
                Address = request.Address,
                Pattern = request.Pattern,
                DayOfWeek = request.DayOfWeek,
                DayOfMonth = request.DayOfMonth,
                TimeOfDay = request.TimeOfDay,
                DurationMinutes = request.DurationMinutes,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = RecurringBookingStatus.PendingApproval,
                CreatedAt = DateTime.UtcNow
            };

            await _recurringBookingRepository.AddAsync(recurringBooking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return recurringBooking.Id;
        }
    }
}
