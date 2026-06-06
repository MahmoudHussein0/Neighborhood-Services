using MediatR;
using Neighborhood.Services.Application.Auth.DTOs;
using Neighborhood.Services.Application.Users.Interfaces;

namespace Neighborhood.Services.Application.Auth.Commands
{
    public class ResetPasswordCommandHandler(IUserRepository userRepository) : IRequestHandler<ResetPasswordCommand, ResetPasswordResponseDTO>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<ResetPasswordResponseDTO> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var result = await _userRepository.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                return new ResetPasswordResponseDTO
                {
                    Succeeded = false,
                    Message = "Password reset failed",
                    Errors = result.Errors.Select(error => error.Description).ToList()
                };
            }

            return new ResetPasswordResponseDTO
            {
                Succeeded = true,
                Message = "Password reset successfully"
            };
        }
    }
}
