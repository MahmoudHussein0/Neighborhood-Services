using MediatR;
using Neighborhood.Services.Application.Exceptions;
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
        public SetRecurringPriceCommandHandler(
             IRecurringBookingRepository recurringBookingRepository,
             ITechnicianRepository technicianRepository,
             ICurrentUserService currentUserService,
             IUnitOfWork unitOfWork)
        {
            _recurringBookingRepository = recurringBookingRepository;
            _technicianRepository = technicianRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
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

            return true;
        }
    }
}
