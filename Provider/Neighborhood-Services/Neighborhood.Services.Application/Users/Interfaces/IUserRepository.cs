using Microsoft.AspNetCore.Identity;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUsers;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Application.Users.Interfaces
{
    public interface IUserRepository : IGenericRepository<ApplicationUser, string>
    {
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<List<ApplicationUser>> GetUsersByRoleAsync(ApplicationUserRole role);
        Task<List<ApplicationUser>> GetNearbyUsersAsync(Point location, double distanceInMeters);
    }
}
