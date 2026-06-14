using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.Staffs.Repository
{
    /// <summary>
    /// Repository implementation for Staff entity
    /// Includes ApplicationUser data for FullName and Email in all queries
    /// </summary>
    public class StaffRepository : GenericRepository<Staff, int>, IStaffRepository
    {
        private readonly ApplicationDbContext _context;

        public StaffRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all staff with ApplicationUser data included
        /// </summary>
        public async Task<IEnumerable<Staff>> GetAllAsync()
        {
            return await _context.Staffs
                .Include(s => s.User)
                .Include(s => s.Permissions)
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }

        /// <summary>
        /// Get all active staff with ApplicationUser data included
        /// </summary>
        public async Task<IEnumerable<Staff>> GetActiveAsync()
        {
            return await _context.Staffs
                .Include(s => s.User)
                .Include(s => s.Permissions)
                .Where(s => s.IsActive && !s.IsDeleted)
                .ToListAsync();
        }

        /// <summary>
        /// Get staff by ID with ApplicationUser data included
        /// </summary>
        public async Task<Staff> GetByIdAsync(int id)
        {
            return await _context.Staffs
                .Include(s => s.User)
                .Include(s => s.Permissions)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        /// <summary>
        /// Get staff by User ID with ApplicationUser data included
        /// </summary>
        public async Task<Staff> GetByUserIdAsync(string userId)
        {
            return await _context.Staffs
                .Include(s => s.User)
                .Include(s => s.Permissions)
                .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted);
        }

        /// <summary>
        /// Get staff members by role with ApplicationUser data included
        /// </summary>
        public async Task<IEnumerable<Staff>> GetByRoleAsync(StaffRole role)
        {
            return await _context.Staffs
                .Include(s => s.User)
                 .Include(s => s.Permissions)
                .Where(s => s.Role == role && !s.IsDeleted)
                .ToListAsync();
        }

     
        public async Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Staffs
                .AnyAsync(s => s.UserId == userId, cancellationToken);
        }

        public async Task<bool> HasPermissionAsync(int staffId, PermissionType permission, CancellationToken cancellationToken = default)
        {
            return await _context.StaffPermissions
                .AnyAsync(
                    p => p.StaffId == staffId &&
                         (p.Permission == permission || p.Permission == PermissionType.FullAccess),
                    cancellationToken);
        }


        public async Task ReplacePermissionsAsync(
     int staffId,
     IEnumerable<PermissionType> permissions,
     CancellationToken cancellationToken = default)
        {
            var existingPermissions = await _context.StaffPermissions
                .Where(x => x.StaffId == staffId && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var permission in existingPermissions)
            {
                permission.IsDeleted = true;
            }

            foreach (var permission in permissions.Distinct())
            {
                await _context.StaffPermissions.AddAsync(
                    new StaffPermission
                    {
                        StaffId = staffId,
                        Permission = permission
                    },
                    cancellationToken);
            }
        }

    }
}
