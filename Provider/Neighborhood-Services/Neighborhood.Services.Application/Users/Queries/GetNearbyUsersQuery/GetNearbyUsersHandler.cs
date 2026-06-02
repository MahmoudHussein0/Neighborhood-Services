using MediatR;
using Neighborhood.Services.Application.Users.DTOs;
using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Application.Users.Queries
{
    public class GetNearbyUsersHandler(IUserRepository userRepository) : IRequestHandler<GetNearbyUsersQuery, List<UserSummaryDTO>>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<List<UserSummaryDTO>> Handle(GetNearbyUsersQuery request, CancellationToken cancellationToken)
        {
            var location = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
            var users = await _userRepository.GetNearbyUsersAsync(location, request.DistanceInMeters);

            return users.Select(MapToSummary).ToList();
        }

        private static UserSummaryDTO MapToSummary(ApplicationUser user)
        {
            return new UserSummaryDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Photo = user.Photo,
                IsActive = user.IsActive,
                ApplicationUserRole = user.ApplicationUserRole
            };
        }
    }
}
