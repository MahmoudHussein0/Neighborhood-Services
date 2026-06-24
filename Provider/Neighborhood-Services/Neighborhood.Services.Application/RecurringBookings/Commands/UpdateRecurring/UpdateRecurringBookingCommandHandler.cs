using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.RecurringBookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.UpdateRecurring
{
    public class UpdateRecurringBookingCommandHandler : IRequestHandler<UpdateRecurringBookingCommand,bool>
    {
        private readonly IRecurringBookingRepository _recurringBookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<UpdateRecurringBookingCommandHandler> _logger;

        public UpdateRecurringBookingCommandHandler(
            IRecurringBookingRepository recurringBookingRepository,
            ICustomerRepository customerRepository,
            ITechnicianRepository technicianRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ILogger<UpdateRecurringBookingCommandHandler> logger)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _customerRepository = customerRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(UpdateRecurringBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var recurringBooking = await _recurringBookingRepository
                .GetByIdAsync(request.RecurringBookingId);

            if (recurringBooking is null)
                throw new NotFoundException(nameof(RecurringBooking), request.RecurringBookingId);

            // Only customer can update
            var customer = await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Customer", userId);

            if (recurringBooking.CustomerId != customer.Id)
                throw new ForbiddenException("You don't have access to this recurring booking.");

            if (recurringBooking.Status == RecurringBookingStatus.Cancelled)
                throw new BadRequestException("Cannot update a cancelled recurring booking.");

            // Validate pattern fields
            if (request.Pattern == RecurringPattern.Weekly && request.DayOfWeek == null)
                throw new ValidationException("Day of week is required for weekly pattern.");

            if (request.Pattern == RecurringPattern.Monthly && request.DayOfMonth == null)
                throw new ValidationException("Day of month is required for monthly pattern.");

            if (request.EndDate.HasValue && request.EndDate <= request.StartDate)
                throw new ValidationException("End date must be after start date.");

            if (request.DurationMinutes <= 0)
                throw new ValidationException("Duration must be greater than zero.");

            if (string.IsNullOrWhiteSpace(request.Description))
                throw new ValidationException("Description is required.");

            recurringBooking.Description = request.Description.Trim();
            recurringBooking.ImageUrl = request.ImageUrl;
            recurringBooking.Address = request.Address;
            recurringBooking.Pattern = request.Pattern;
            recurringBooking.DayOfWeek = request.DayOfWeek;
            recurringBooking.DayOfMonth = request.DayOfMonth;
            recurringBooking.TimeOfDay = request.TimeOfDay;
            recurringBooking.DurationMinutes = request.DurationMinutes;
            recurringBooking.StartDate = request.StartDate;
            recurringBooking.EndDate = request.EndDate;

            // Reset to PendingApproval — technician must re-approve after changes
            recurringBooking.AgreedPrice = null;
            recurringBooking.Status = RecurringBookingStatus.PendingApproval;

            await _recurringBookingRepository.UpdateAsync(recurringBooking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // The edit reset the arrangement to PendingApproval, so the technician must re-price.
            // Let them know. Best effort — a failed notification must not fail the update.
            try
            {
                var technician = await _technicianRepository.GetByIdAsync(recurringBooking.TechnicianId);
                if (!string.IsNullOrEmpty(technician?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        technician.ApplicationUserId,
                        $"Recurring booking #{recurringBooking.Id} was updated — open Recurring Jobs to review and set a price.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Update-recurring-booking notification failed for recurring booking {Id}.", recurringBooking.Id);
            }

            return true;
        }
    }
}
