using MediatR;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class ActivateUserCommand : IRequest
    {
        public string Id { get; set; } = string.Empty;
    }
}
