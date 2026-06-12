using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Application.Staffs.Interfaces
{
    /// <summary>
    /// Repository interface for Staff entity
    /// NOTE: All methods should eagerly load ApplicationUser using .Include(s => s.ApplicationUser)
    /// to retrieve FullName and Email for DTO mapping
    /// </summary>
    public interface IStaffRepository : IGenericRepository<Staff, int>  
    {
        /// <summary>
        /// Get all staff members with ApplicationUser data included
        /// </summary>
        Task<IEnumerable<Staff>> GetAllAsync();

        /// <summary>
        /// Get all active staff members with ApplicationUser data included
        /// </summary>
        Task<IEnumerable<Staff>> GetActiveAsync();

        /// <summary>
        /// Get staff by ID with ApplicationUser data included
        /// </summary>
        Task<Staff> GetByIdAsync(int id);

        /// <summary>
        /// Get staff by User ID with ApplicationUser data included
        /// </summary>
        Task<Staff> GetByUserIdAsync(string userId);

        /// <summary>
        /// Get staff members by role with ApplicationUser data included
        /// </summary>
        Task<IEnumerable<Staff>> GetByRoleAsync(StaffRole role);

        // ── Queries ────────────────────────────────────────────────────────────
        
     
        Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<bool> HasPermissionAsync(int staffId, PermissionType permission, CancellationToken cancellationToken = default);

        // ── Commands ───────────────────────────────────────────────────────────

        Task ReplacePermissionsAsync(int staffId, IEnumerable<PermissionType> permissions, CancellationToken cancellationToken = default);


     
    }
}
