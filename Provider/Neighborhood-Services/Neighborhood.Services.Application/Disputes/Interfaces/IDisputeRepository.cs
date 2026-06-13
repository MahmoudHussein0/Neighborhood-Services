using Neighborhood.Services.Application.Disputes.DTOs;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Disputes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.Disputes.Interfaces
{
    public interface IDisputeRepository : IGenericRepository<Dispute, int>
    {
        // ── Queries ────────────────────────────────────────────────────────────

        // Loads a single dispute already projected with the booking's two parties
        // (customer/technician user ids + names) and escrow, for the staff details modal.
        Task<DisputeDto?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Dispute>> GetByStatusAsync(DisputeStatus status, CancellationToken cancellationToken = default);
        Task<Dispute?> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Dispute>> GetByRaisedByAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Dispute>> GetByResolvedByStaffIdAsync(int staffId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Dispute>> GetByTypeAsync(DisputeType type, CancellationToken cancellationToken = default);
        Task<bool> ExistsByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);

     
       
    }
}
