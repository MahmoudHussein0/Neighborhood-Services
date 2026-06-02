using MediatR;
using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Domain.Disputes;

namespace Neighborhood.Services.Application.Disputes.Commands
{
    public class UpdateDisputeCommand : IRequest<DisputeDto>
    {
        public int Id { get; set; }
        public DisputeStatus Status { get; set; }
        public int? ResolvedByStaffId { get; set; }
        public string? Resolution { get; set; }
    }

}
