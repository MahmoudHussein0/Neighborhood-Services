using MediatR;
using Neighborhood.Services.Application.Users.DTOs;
using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;

namespace Neighborhood.Services.Application.Users.Queries
{
    public class GetUsersByRoleHandler(IUserRepository userRepository) : IRequestHandler<GetUsersByRoleQuery, List<UserSummaryDTO>>
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<List<UserSummaryDTO>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetUsersByRoleAsync(request.ApplicationUserRole);

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
