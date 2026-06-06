using MediatR;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ServiceRequests;

namespace Neighborhood.Services.Application.ServiceRequests.Commands.CloseService
{
    public class CloseServiceRequestCommandHandler : IRequestHandler<CloseServiceRequestCommand, bool>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CloseServiceRequestCommandHandler(IServiceRequestRepository serviceRequestRepository, IUnitOfWork unitOfWork)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(CloseServiceRequestCommand request, CancellationToken cancellationToken)
        {
            // TODO: Authorization check once current user service is ready
            // Only the customer who created this request can close it manually
            var serviceRequest = await _serviceRequestRepository.GetByIdAsync(request.ServiceRequestId);

            if (serviceRequest is null)
                throw new NotFoundException(nameof(ServiceRequest), request.ServiceRequestId);

            if (serviceRequest.Status == ServiceRequestStatus.Closed || serviceRequest.Status == ServiceRequestStatus.Expired)
                throw new BadRequestException($"Service request is already {serviceRequest.Status}.");

            serviceRequest.Status = ServiceRequestStatus.Closed;

            await _serviceRequestRepository.UpdateAsync(serviceRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
