using MediatR;

namespace Neighborhood.Services.Application.ServiceRequests.Commands.CreateService
{
    public class CreateServiceRequestCommand : IRequest<int>
    {
        // CustomerId is resolved from the authenticated user in the handler.
        public int CategoryId { get; set; }
        public int ProblemTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public string? Image { get; set; }
        public DateTime ScheduledAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
