using MediatR;
using Neighborhood.Services.Application.Users.Interfaces;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class UpdateUserPhotoCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserPhotoCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(UpdateUserPhotoCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Photo = request.Photo;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }
    }
}
