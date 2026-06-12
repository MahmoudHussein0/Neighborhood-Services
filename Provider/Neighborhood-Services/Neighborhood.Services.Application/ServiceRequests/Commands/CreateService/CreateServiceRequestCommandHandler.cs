using MediatR;
using Neighborhood.Services.Application.Customers.Interfaces;
using Neighborhood.Services.Application.Exceptions;
using Neighborhood.Services.Application.ServiceRequests.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ServiceRequests;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Application.ServiceRequests.Commands.CreateService
{
    public class CreateServiceRequestCommandHandler : IRequestHandler<CreateServiceRequestCommand, int>
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IBackgroundJobScheduler _backgroundJobs;

        public CreateServiceRequestCommandHandler(IServiceRequestRepository serviceRequestRepository, IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ICustomerRepository customerRepository, IBackgroundJobScheduler backgroundJobs)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _customerRepository = customerRepository;
            _backgroundJobs = backgroundJobs;
        }

        public async Task<int> Handle(CreateServiceRequestCommand request, CancellationToken cancellationToken)
        {
            // Budget must be positive
            if (request.Budget <= 0)
                throw new ValidationException("Budget must be greater than zero");

            // Description must not be empty
            if (string.IsNullOrWhiteSpace(request.Description))
                throw new BadRequestException("Description is required");

            // Desired service time must be in the future
            if (request.ScheduledAt <= DateTime.UtcNow)
                throw new ValidationException("Scheduled time cannot be in the past");

            // Resolve the customer from the authenticated user
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedException("User is not authenticated.");
            var customer = await _customerRepository.GetByUserIdAsync(userId)
                ?? throw new NotFoundException("Customer", userId);

            var serviceRequest = new ServiceRequest
            {
                CustomerId = customer.Id,
                CategoryId = request.CategoryId,
                ProblemTypeId = request.ProblemTypeId,
                Description = request.Description,
                Address = request.Address,
                Budget = request.Budget,
                Image = request.Image,
                ScheduledAt = request.ScheduledAt,
                // Created in PendingReview — the moderation agent flips it to Open/Flagged
                // in the background. Never start as Open: that would skip moderation.
                Status = ServiceRequestStatus.PendingReview,
                Location = new Point(request.Longitude, request.Latitude) { SRID = 4326 },
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _serviceRequestRepository.AddAsync(serviceRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Queue background moderation. This is just an INSERT into Hangfire's table
            // (microseconds) — the user does not wait for the AI call.
            _backgroundJobs.EnqueueServiceRequestModeration(serviceRequest.Id);

            return serviceRequest.Id;
        }
    }
}
