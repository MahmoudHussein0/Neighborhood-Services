using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
using NetTopologySuite.Geometries;

namespace Neighborhood.Services.Infrastructure.Persistence.Users
{
    public class UserRepository : GenericRepository<ApplicationUser, string>, IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public override async Task UpdateAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(error => error.Description)));
            }
        }

        public async Task<List<ApplicationUser>> GetUsersByRoleAsync(ApplicationUserRole role)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(user => user.ApplicationUserRole == role && !user.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetNearbyUsersAsync(Point location, double distanceInMeters)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(user => !user.IsDeleted && user.Location != null && user.Location.Distance(location) <= distanceInMeters)
                .ToListAsync();
        }
    }
}
