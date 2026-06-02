using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Technicians.Interfaces;
using Neighborhood.Services.Domain.RecurringBookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.CancelRecurring
{
    public class CancelRecurringBookingCommandHandler
    {
        private readonly IRecurringBookingRepository _recurringBookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ITechnicianRepository _technicianRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public CancelRecurringBookingCommandHandler(
            IRecurringBookingRepository recurringBookingRepository,
            ICustomerRepository customerRepository,
            ITechnicianRepository technicianRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _customerRepository = customerRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
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

            return true;
        }
    }
}
