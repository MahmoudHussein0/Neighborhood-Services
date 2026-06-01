using MediatR;
using Neighborhood.Services.Application.Users.Interfaces;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class ActivateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<ActivateUserCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }
    }
}
