using MediatR;
using Neighborhood.Services.Application.Auth.DTOs;

namespace Neighborhood.Services.Application.Auth.Commands
{
    public class ForgotPasswordCommand : IRequest<ForgotPasswordResponseDTO>
    {
        public string Email { get; set; } = string.Empty;
    }
}
