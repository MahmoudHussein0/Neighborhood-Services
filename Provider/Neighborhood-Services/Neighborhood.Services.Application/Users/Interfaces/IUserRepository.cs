using Microsoft.AspNetCore.Identity;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.ApplicationUsers;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Application.Users.Interfaces
{
    public interface IUserRepository : IGenericRepository<ApplicationUser, string>
    {
        Task<IdentityResult> CreateAsync(ApplicationUser user, string password);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
        Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
        Task<List<ApplicationUser>> GetUsersByRoleAsync(ApplicationUserRole role);
        Task<List<ApplicationUser>> GetNearbyUsersAsync(Point location, double distanceInMeters);
        Task<string?> GetTechnicianPhotoAsync(int techId);

    }
}
