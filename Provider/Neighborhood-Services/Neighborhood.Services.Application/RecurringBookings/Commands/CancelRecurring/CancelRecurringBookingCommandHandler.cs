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

namespace Neighborhood.Services.Application.RecurringBookings.Commands.CancelRecurring
{
    public class CancelRecurringBookingCommandHandler : IRequestHandler<CancelRecurringBookingCommand, bool>
    {
        private readonly IRecurringBookingRepository _recurringBookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<CancelRecurringBookingCommandHandler> _logger;

        public CancelRecurringBookingCommandHandler(
            IRecurringBookingRepository recurringBookingRepository,
            ICustomerRepository customerRepository,
            ITechnicianRepository technicianRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ILogger<CancelRecurringBookingCommandHandler> logger)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _customerRepository = customerRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(CancelRecurringBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var recurringBooking = await _recurringBookingRepository
                .GetByIdAsync(request.RecurringBookingId);

            if (recurringBooking is null)
                throw new NotFoundException(nameof(RecurringBooking), request.RecurringBookingId);

            if (recurringBooking.Status == RecurringBookingStatus.Cancelled)
                throw new BadRequestException("Recurring booking is already cancelled.");

            // Check if current user is the customer or technician
            var customer = await _customerRepository.GetByUserIdAsync(userId);
            var technician = await _technicianRepository.GetByUserIdAsync(userId);

            var isCancelledByCustomer = customer != null && recurringBooking.CustomerId == customer.Id;
            var isCancelledByTechnician = technician != null && recurringBooking.TechnicianId == technician.Id;

            if (!isCancelledByCustomer && !isCancelledByTechnician)
                throw new ForbiddenException("You don't have access to this recurring booking.");

            recurringBooking.Status = RecurringBookingStatus.Cancelled;
            recurringBooking.CancelledBy = userId;
            recurringBooking.CancelledAt = DateTime.UtcNow;

            await _recurringBookingRepository.UpdateAsync(recurringBooking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the other party that the recurring booking was cancelled, and remind the
            // customer to check My Bookings — occurrences already generated for the next few days
            // stay live (with their payment held) until cancelled individually. Best effort.
            try
            {
                string? customerUserId = isCancelledByCustomer
                    ? userId
                    : (await _customerRepository.GetByIdAsync(recurringBooking.CustomerId))?.ApplicationUserId;
                string? technicianUserId = isCancelledByTechnician
                    ? userId
                    : (await _technicianRepository.GetByIdAsync(recurringBooking.TechnicianId))?.ApplicationUserId;

                var otherUserId = isCancelledByCustomer ? technicianUserId : customerUserId;
                if (!string.IsNullOrEmpty(otherUserId))
                    await _notificationService.SendNotificationToUser(
                        otherUserId,
                        $"Recurring booking #{recurringBooking.Id} was cancelled.");

                if (!string.IsNullOrEmpty(customerUserId))
                    await _notificationService.SendNotificationToUser(
                        customerUserId,
                        $"Recurring booking #{recurringBooking.Id} was cancelled. Please check My Bookings — any visits already scheduled for the next few days are still booked, so cancel them there if you don't want them.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Cancel-recurring-booking notification failed for recurring booking {Id}.", recurringBooking.Id);
            }

            return true;
        }
    }
}
