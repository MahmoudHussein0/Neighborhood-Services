using MediatR;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class DeactivateUserCommand : IRequest
    {
        public string Id { get; set; } = string.Empty;
    }
}
