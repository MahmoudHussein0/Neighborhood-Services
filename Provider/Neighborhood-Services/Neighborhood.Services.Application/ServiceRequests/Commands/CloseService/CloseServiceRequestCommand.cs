using MediatR;

namespace Neighborhood.Services.Application.ServiceRequests.Commands.CloseService
{
    public class CloseServiceRequestCommand : IRequest<bool>
    {
        public int ServiceRequestId { get; set; }
    }
}
