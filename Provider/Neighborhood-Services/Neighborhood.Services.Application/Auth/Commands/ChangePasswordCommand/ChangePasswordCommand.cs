using MediatR;
using Neighborhood.Services.Application.Auth.DTOs;

namespace Neighborhood.Services.Application.Auth.Commands
{
    public class ChangePasswordCommand : IRequest<ChangePasswordResponseDTO>
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
