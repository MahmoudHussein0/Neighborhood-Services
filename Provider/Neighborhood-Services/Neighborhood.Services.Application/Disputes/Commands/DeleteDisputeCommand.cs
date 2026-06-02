using MediatR;

namespace Neighborhood.Services.Application.Disputes.Commands
{
    public class DeleteDisputeCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
