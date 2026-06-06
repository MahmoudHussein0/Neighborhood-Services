using MediatR;
using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Domain.Disputes;

namespace Neighborhood.Services.Application.Disputes.Commands
{
    public class CreateDisputeCommand : IRequest<DisputeDto>
    {
        public int BookingId { get; set; }
        public int RaisedBy { get; set; }
        public DisputeType DisputeType { get; set; }
        public string Reason { get; set; }
    }
}
