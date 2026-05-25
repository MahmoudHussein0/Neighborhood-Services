using Neighborhood.Services.Domain.Disputes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Disputes.Interfaces
{
    public interface IDisputeRepository
    {
        // ── Queries ────────────────────────────────────────────────────────────
        Task<Dispute?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Dispute>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Dispute>> GetByStatusAsync(DisputeStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Dispute>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Dispute>> GetByRaisedByAsync(int userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Dispute>> GetByResolvedByStaffIdAsync(int staffId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Dispute>> GetByTypeAsync(DisputeType type, CancellationToken cancellationToken = default);
        Task<bool> ExistsByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);

        // ── Commands ───────────────────────────────────────────────────────────
        Task AddAsync(Dispute dispute, CancellationToken cancellationToken = default);
        Task UpdateAsync(Dispute dispute, CancellationToken cancellationToken = default);
        Task DeleteAsync(Dispute dispute, CancellationToken cancellationToken = default);
    }
}
