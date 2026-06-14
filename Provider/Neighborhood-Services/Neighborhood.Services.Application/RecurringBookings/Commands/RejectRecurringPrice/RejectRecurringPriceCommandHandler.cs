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

namespace Neighborhood.Services.Application.RecurringBookings.Commands.RejectRecurringPrice
{
    public class RejectRecurringPriceCommandHandler : IRequestHandler<RejectRecurringPriceCommand,bool>
    {
        private readonly IRecurringBookingRepository _recurringBookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<RejectRecurringPriceCommandHandler> _logger;

        public RejectRecurringPriceCommandHandler(
            IRecurringBookingRepository recurringBookingRepository,
            ICustomerRepository customerRepository,
            ITechnicianRepository technicianRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ILogger<RejectRecurringPriceCommandHandler> logger)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _customerRepository = customerRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(RejectRecurringPriceCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var recurringBooking = await _recurringBookingRepository
                .GetByIdAsync(request.RecurringBookingId);

            if (recurringBooking is null)
                throw new NotFoundException(nameof(RecurringBooking), request.RecurringBookingId);

            // Only the customer who created this recurring booking can reject
            var customer = await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Customer", userId);

            if (recurringBooking.CustomerId != customer.Id)
                throw new ForbiddenException("You don't have access to this recurring booking.");

            if (recurringBooking.Status != RecurringBookingStatus.PendingCustomerApproval)
                throw new BadRequestException($"Cannot reject. Current status: {recurringBooking.Status}.");

            // Reset back to PendingApproval — technician can set a new price
            recurringBooking.AgreedPrice = null;
            recurringBooking.Status = RecurringBookingStatus.PendingApproval;

            await _recurringBookingRepository.UpdateAsync(recurringBooking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the technician that their proposed price was rejected — best effort.
            try
            {
                var technician = await _technicianRepository.GetByIdAsync(recurringBooking.TechnicianId);
                if (!string.IsNullOrEmpty(technician?.ApplicationUserId))
                    await _notificationService.SendNotificationToUser(
                        technician.ApplicationUserId,
                        $"The customer rejected the price for recurring booking #{recurringBooking.Id}. You can set a new price.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Reject-recurring-price notification failed for recurring booking {Id}.", recurringBooking.Id);
            }

            return true;
        }
    }
}
