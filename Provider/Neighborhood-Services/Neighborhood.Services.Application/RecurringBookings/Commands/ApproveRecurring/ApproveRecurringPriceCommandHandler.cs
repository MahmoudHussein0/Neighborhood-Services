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

namespace Neighborhood.Services.Application.RecurringBookings.Commands.ApproveRecurring
{
    public class ApproveRecurringPriceCommandHandler:IRequestHandler<ApproveRecurringPriceCommand,bool>
    {
        private readonly IRecurringBookingRepository _recurringBookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ApproveRecurringPriceCommandHandler> _logger;

        public ApproveRecurringPriceCommandHandler(
            IRecurringBookingRepository recurringBookingRepository,
            ICustomerRepository customerRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ITechnicianRepository technicianRepository,
            INotificationService notificationService,
            ILogger<ApproveRecurringPriceCommandHandler> logger)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _technicianRepository = technicianRepository;
            _notificationService = notificationService;
            _logger = logger;
        }
        public async Task<bool> Handle(ApproveRecurringPriceCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var recurringBooking = await _recurringBookingRepository
                .GetByIdAsync(request.RecurringBookingId);

            if (recurringBooking is null)
                throw new NotFoundException(nameof(RecurringBooking), request.RecurringBookingId);

            // Only the customer who created this recurring booking can approve
            var customer = await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Customer", userId);

            if (recurringBooking.CustomerId != customer.Id)
                throw new ForbiddenException("You don't have access to this recurring booking.");

            if (recurringBooking.Status != RecurringBookingStatus.PendingCustomerApproval)
                throw new BadRequestException($"Cannot approve. Current status: {recurringBooking.Status}.");

            recurringBooking.Status = RecurringBookingStatus.Active;

            await _recurringBookingRepository.UpdateAsync(recurringBooking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the technician the customer approved the price — best effort.
            try
            {
                var technician = await _technicianRepository.GetByIdAsync(recurringBooking.TechnicianId);
                if (!string.IsNullOrEmpty(technician?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        technician.ApplicationUserId,
                        $"The customer approved the price for recurring booking #{recurringBooking.Id}. It's now active.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Approve-recurring notification failed for recurring booking {Id}.", recurringBooking.Id);
            }

            return true;
        }
    }
}
