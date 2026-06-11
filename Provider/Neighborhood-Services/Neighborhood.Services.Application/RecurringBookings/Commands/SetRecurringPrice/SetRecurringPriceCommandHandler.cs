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

namespace Neighborhood.Services.Application.RecurringBookings.Commands.SetRecurringPrice
{
    public class SetRecurringPriceCommandHandler:IRequestHandler<SetRecurringPriceCommand,bool>
    {
        private readonly IRecurringBookingRepository _recurringBookingRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<SetRecurringPriceCommandHandler> _logger;
        public SetRecurringPriceCommandHandler(
             IRecurringBookingRepository recurringBookingRepository,
             ITechnicianRepository technicianRepository,
             ICurrentUserService currentUserService,
             IUnitOfWork unitOfWork,
             ICustomerRepository customerRepository,
             INotificationService notificationService,
             ILogger<SetRecurringPriceCommandHandler> logger)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(SetRecurringPriceCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            if (request.Price <= 0)
                throw new ValidationException("Price must be greater than zero.");

            var recurringBooking = await _recurringBookingRepository
                .GetByIdAsync(request.RecurringBookingId);

            if (recurringBooking is null)
                throw new NotFoundException(nameof(RecurringBooking), request.RecurringBookingId);

            // Only the assigned technician can set the price
            var technician = await _technicianRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Technician", userId);

            if (recurringBooking.TechnicianId != technician.Id)
                throw new ForbiddenException("You don't have access to this recurring booking.");

            if (recurringBooking.Status != RecurringBookingStatus.PendingApproval)
                throw new BadRequestException($"Cannot set price. Current status: {recurringBooking.Status}.");

            recurringBooking.AgreedPrice = request.Price;
            recurringBooking.Status = RecurringBookingStatus.PendingCustomerApproval;

            await _recurringBookingRepository.UpdateAsync(recurringBooking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the customer a price was proposed and awaits their approval — best effort.
            try
            {
                var customer = await _customerRepository.GetByIdAsync(recurringBooking.CustomerId);
                if (!string.IsNullOrEmpty(customer?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        customer.ApplicationUserId,
                        $"Your technician set a price for recurring booking #{recurringBooking.Id}. Review and approve to activate it.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Set-recurring-price notification failed for recurring booking {Id}.", recurringBooking.Id);
            }

            return true;
        }
    }
}
