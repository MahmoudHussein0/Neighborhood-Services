using MediatR;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class DeleteUserCommand : IRequest
    {
        public string Id { get; set; } = string.Empty;
    }
}
