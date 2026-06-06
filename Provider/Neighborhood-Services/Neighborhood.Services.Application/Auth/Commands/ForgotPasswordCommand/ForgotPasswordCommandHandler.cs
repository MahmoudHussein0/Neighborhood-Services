using MediatR;
using Neighborhood.Services.Application.Auth.DTOs;
using Neighborhood.Services.Application.Users.Interfaces;

namespace Neighborhood.Services.Application.Auth.Commands
{
    public class ForgotPasswordCommandHandler(IUserRepository userRepository) : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponseDTO>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<ForgotPasswordResponseDTO> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var response = new ForgotPasswordResponseDTO
            {
                Succeeded = true,
                Message = "If the email exists, password reset instructions have been generated."
            };

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || user.IsDeleted || !user.IsActive)
            {
                return response;
            }

            response.Token = await _userRepository.GeneratePasswordResetTokenAsync(user);
            return response;
        }
    }
}
