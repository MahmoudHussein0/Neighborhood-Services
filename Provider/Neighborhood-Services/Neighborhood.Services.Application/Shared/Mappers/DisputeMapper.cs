using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Domain.Disputes;

namespace Neighborhood.Services.Application.Shared.Mappers
{
    public static class DisputeMapper
    {
        public static DisputeDto MapToDto(Dispute dispute) => new()
        {
            Id = dispute.Id,
            BookingId = dispute.BookingId,
            RaisedBy = dispute.RaisedBy,
            ResolvedByStaffId = dispute.ResolvedByStaffId,
            DisputeType = dispute.DisputeType.ToString(),
            Reason = dispute.Reason,
            Resolution = dispute.Resolution,
            Status = dispute.Status.ToString(),
            CreatedAt = dispute.CreatedAt,
            ResolvedAt = dispute.ResolvedAt
        };
    }
}
