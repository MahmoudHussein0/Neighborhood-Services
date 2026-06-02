using MediatR;

namespace Neighborhood.Services.Application.CancellationPolicies.Commands.DeleteCancellation
{
    public class DeleteCancellationPolicyCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
