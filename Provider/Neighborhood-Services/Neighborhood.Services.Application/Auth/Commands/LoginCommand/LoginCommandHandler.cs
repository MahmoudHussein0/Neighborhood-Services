using MediatR;
using Neighborhood.Services.Application.Auth.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Users.Interfaces;

namespace Neighborhood.Services.Application.Auth.Commands
{
    public class LoginCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, AuthResponseDTO>
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJwtTokenService _jwtTokenService = jwtTokenService;

        public async Task<AuthResponseDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (user.IsDeleted || !user.IsActive)
            {
                throw new UnauthorizedAccessException("User account is not active");
            }

            var isPasswordValid = await _userRepository.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var tokenResult = _jwtTokenService.GenerateToken(user);

            return new AuthResponseDTO
            {
                Token = tokenResult.Token,
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Photo = user.Photo,
                Role = user.ApplicationUserRole.ToString(),
                ExpiresAt = tokenResult.ExpiresAt
            };
        }
    }
}
