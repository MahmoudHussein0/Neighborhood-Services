using MediatR;
using Neighborhood.Services.Application.Users.Interfaces;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Application.Users.Commands
{
    public class UpdateUserLocationCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserLocationCommand>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task Handle(UpdateUserLocationCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Location = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
        }
    }
}
