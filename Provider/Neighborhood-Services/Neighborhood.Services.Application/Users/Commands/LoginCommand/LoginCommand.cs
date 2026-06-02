using MediatR;
using Neighborhood.Services.Application.Users.DTOs;

namespace Neighborhood.Services.Application.Users.Commands.LoginCommand
{
    public class LoginCommand : IRequest<AuthResponseDTO>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
