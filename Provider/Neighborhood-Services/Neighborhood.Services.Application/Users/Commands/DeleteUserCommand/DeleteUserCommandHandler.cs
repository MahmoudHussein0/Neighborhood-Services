using MediatR;
using Neighborhood.Services.Application.Users.Interfaces;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class DeleteUserCommandHandler(IUserRepository userRepository) : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.IsDeleted = true;
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }
    }
}
