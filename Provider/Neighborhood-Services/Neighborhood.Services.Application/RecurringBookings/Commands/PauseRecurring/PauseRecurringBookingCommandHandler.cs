using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.RecurringBookings.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.RecurringBookings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.RecurringBookings.Commands.PauseRecurring
{
    public class PauseRecurringBookingCommandHandler:IRequestHandler<PauseRecurringBookingCommand,bool>
    {
        private readonly IRecurringBookingRepository _recurringBookingRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public PauseRecurringBookingCommandHandler(
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

        public async Task<bool> Handle(PauseRecurringBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");

            var recurringBooking = await _recurringBookingRepository
                .GetByIdAsync(request.RecurringBookingId);

            if (recurringBooking is null)
                throw new NotFoundException(nameof(RecurringBooking), request.RecurringBookingId);

            // Only customer can pause
            var customer = await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Customer", userId);

            if (recurringBooking.CustomerId != customer.Id)
                throw new ForbiddenException("You don't have access to this recurring booking.");

            if (recurringBooking.Status != RecurringBookingStatus.Active)
                throw new BadRequestException("Only an active recurring booking can be paused.");

            recurringBooking.Status = RecurringBookingStatus.Paused;

            await _recurringBookingRepository.UpdateAsync(recurringBooking);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
