using MediatR;
using Neighborhood.Services.Application.Auth.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Users.Interfaces;

namespace Neighborhood.Services.Application.Auth.Commands
{
    public class ChangePasswordCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService) : IRequestHandler<ChangePasswordCommand, ChangePasswordResponseDTO>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICurrentUserService _currentUserService = currentUserService;

        public async Task<ChangePasswordResponseDTO> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var result = await _userRepository.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                return new ChangePasswordResponseDTO
                {
                    Succeeded = false,
                    Message = "Password change failed",
                    Errors = result.Errors.Select(error => error.Description).ToList()
                };
            }

            return new ChangePasswordResponseDTO
            {
                Succeeded = true,
                Message = "Password changed successfully"
            };
        }
    }
}
