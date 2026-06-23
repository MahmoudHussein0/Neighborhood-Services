using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Application.Shared;
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
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRecurringBookingCommandHandler(
            IRecurringBookingRepository recurringBookingRepository,
            ICustomerRepository customerRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
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

            return true;
        }
    }
}
