using MediatR;
using Microsoft.Extensions.Logging;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.Notifications.Services;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.ServiceRequests.Commands.ReviewFlaggedServiceRequest
{
    public class ReviewFlaggedServiceRequestCommandHandler : IRequestHandler<ReviewFlaggedServiceRequestCommand, bool>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerRepository _customerRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<ReviewFlaggedServiceRequestCommandHandler> _logger;

        public ReviewFlaggedServiceRequestCommandHandler(
            IServiceRequestRepository serviceRequestRepository,
            IUnitOfWork unitOfWork,
            ICustomerRepository customerRepository,
            INotificationService notificationService,
            ILogger<ReviewFlaggedServiceRequestCommandHandler> logger)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _unitOfWork = unitOfWork;
            _customerRepository = customerRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<bool> Handle(ReviewFlaggedServiceRequestCommand request, CancellationToken cancellationToken)
        {
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(request.ServiceRequestId)
                ?? throw new NotFoundException("ServiceRequest", request.ServiceRequestId);

            // Only flagged requests can be reviewed — guards against double-handling.
            if (serviceRequest.Status != ServiceRequestStatus.Flagged)
                throw new BadRequestException("Only flagged requests can be reviewed.");

            serviceRequest.Status = request.Approved
                ? ServiceRequestStatus.Open
                : ServiceRequestStatus.Closed;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Notify the customer of the decision — best effort.
            try
            {
                var customer = await _customerRepository.GetByIdAsync(serviceRequest.CustomerId);
                if (!string.IsNullOrEmpty(customer?.ApplicationUserId))
                {
                    var message = request.Approved
                        ? $"Your service request #{serviceRequest.Id} has been approved and is now live."
                        : $"Your service request #{serviceRequest.Id} couldn't be published. Please review it and post again.";

                    await _notificationService.SendNotificationToUser(customer.ApplicationUserId, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Review notification failed for request {Id}.", serviceRequest.Id);
            }

            return request.Approved;
        }
    }
}
