using Neighborhood.Services.Domain.Staffs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Staffs.Interfaces
{
    public interface IStaffRepository
    {
        // ── Queries ────────────────────────────────────────────────────────────
        Task<Staff?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Staff?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Staff>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Staff>> GetByRoleAsync(StaffRole role, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Staff>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<bool> HasPermissionAsync(int staffId, PermissionType permission, CancellationToken cancellationToken = default);

        // ── Commands ───────────────────────────────────────────────────────────
        Task AddAsync(Staff staff, CancellationToken cancellationToken = default);
        Task UpdateAsync(Staff staff, CancellationToken cancellationToken = default);
        Task DeleteAsync(Staff staff, CancellationToken cancellationToken = default);
    }
}
