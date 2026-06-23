using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Users.Interfaces;
using Neighborhood.Services.Domain.ApplicationUsers;
using Neighborhood.Services.Domain.Technicians;
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

        public async Task<ApplicationUser?> GetByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
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

        //arwa
        public async Task<string?> GetTechnicianPhotoAsync(int techId)
        {
            
            var res = await _context.Users.Include(e => e.Technician).Where(e => e.Technician.Id == techId).FirstOrDefaultAsync();
            return res?.Photo??null!;
        }

        public async Task<List<string>> GetAdminsIdsAsync()
        {

            var res = await _context.Users.Where(e => e.ApplicationUserRole == ApplicationUserRole.Staff).Select(e => e.Id).ToListAsync();
            return res;
        }

        public async Task<List<string>> GetTechniciansIdsAsync()
        {

            var res = await _context.Users.Where(e => e.ApplicationUserRole == ApplicationUserRole.Technician).Select(e => e.Id).ToListAsync();
            return res;
        }

    }
    }

