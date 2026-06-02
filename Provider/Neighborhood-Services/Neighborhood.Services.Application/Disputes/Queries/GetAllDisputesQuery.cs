using MediatR;
using Neighborhood.Services.Application.Disputes.DTOs;

namespace Neighborhood.Services.Application.Disputes.Queries
{
    public class GetAllDisputesQuery : IRequest<IReadOnlyList<DisputeDto>>
    {
    }
}
