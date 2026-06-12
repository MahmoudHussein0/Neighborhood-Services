using MediatR;
using Neighborhood.Services.Application.ServiceRequests.Commands.ModerateService;

namespace Neighborhood.Services.Infrastructure.Services
{
    /// <summary>
    /// Hangfire entry point for moderating a service request. Hangfire resolves this
    /// from DI on a worker thread and calls <see cref="Run"/>. It stays thin — the real
    /// logic lives in the MediatR handler (same as the synchronous AnalyzeBooking agent).
    /// </summary>
    public class ServiceRequestModerationJob
    {
        private readonly IMediator _mediator;

        public ServiceRequestModerationJob(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Run(int serviceRequestId)
            => _mediator.Send(new ModerateServiceRequestCommand { ServiceRequestId = serviceRequestId });
    }
}
