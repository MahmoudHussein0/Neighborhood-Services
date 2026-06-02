using MediatR;
using Neighborhood.Services.Application.Disputes.DTOs;

namespace Neighborhood.Services.Application.Disputes.Queries
{
    public class GetDisputeByIdQuery : IRequest<DisputeDto>
    {
        public int Id { get; set; }
        public GetDisputeByIdQuery(int id) => Id = id;
    }
}
