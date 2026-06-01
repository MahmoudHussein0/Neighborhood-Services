using MediatR;
using Neighborhood.Services.Application.Users.Interfaces;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class DeactivateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }
    }
}
