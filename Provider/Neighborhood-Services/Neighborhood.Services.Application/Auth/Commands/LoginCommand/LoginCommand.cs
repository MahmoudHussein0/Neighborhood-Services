using MediatR;
using Neighborhood.Services.Application.Auth.DTOs;

namespace Neighborhood.Services.Application.Auth.Commands
{
    public class LoginCommand : IRequest<AuthResponseDTO>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
