using MediatR;
using Neighborhood.Services.Application.Offers.DTOs;

namespace Neighborhood.Services.Application.Offers.Commands.CreateOffer
{
    public class CreateOfferCommand : IRequest<CreateOfferResultDto>
    {
        // TechnicianId is resolved from the authenticated user in the handler.
        public int ServiceRequestId { get; set; }
        public decimal Price { get; set; }
        public int EstimatedDuration { get; set; }
        public string Message { get; set; } = string.Empty;
        // Technician's proposed service time
        public DateTime ScheduledAt { get; set; }
    }
}
