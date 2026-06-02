using MediatR;
using Neighborhood.Services.Application.Users.Interfaces;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class UpdateUserProfileCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserProfileCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.FullName = request.FullName;
            user.Age = request.Age;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }
    }
}
