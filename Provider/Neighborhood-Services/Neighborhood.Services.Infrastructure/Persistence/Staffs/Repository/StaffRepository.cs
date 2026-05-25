using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Staffs.Interfaces;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.Infrastructure.Persistence.Staffs.Repository
{
    public class StaffRepository : IStaffRepository
    {
        private readonly IApplicationDbContext _context;

        public StaffRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        // ── Queries ────────────────────────────────────────────────────────────

        public async Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Staffs
                .Include(s => s.Permissions)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Staff?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.Staffs
                .Include(s => s.Permissions)
                .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
        }

        public async Task<IReadOnlyList<Staff>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Staffs
                .Include(s => s.Permissions)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Staff>> GetByRoleAsync(StaffRole role, CancellationToken cancellationToken = default)
        {
            return await _context.Staffs
                .Include(s => s.Permissions)
                .Where(s => s.Role == role)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Staff>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Staffs
                .Include(s => s.Permissions)
                .Where(s => s.IsActive)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
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

        // ── Commands ───────────────────────────────────────────────────────────

        public async Task AddAsync(Staff staff, CancellationToken cancellationToken = default)
        {
            await _context.Staffs.AddAsync(staff, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Staff staff, CancellationToken cancellationToken = default)
        {
            _context.Staffs.Update(staff);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Staff staff, CancellationToken cancellationToken = default)
        {
            _context.Staffs.Remove(staff);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
