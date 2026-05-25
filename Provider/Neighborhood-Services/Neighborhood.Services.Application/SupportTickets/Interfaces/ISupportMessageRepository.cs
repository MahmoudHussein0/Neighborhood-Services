using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Interfaces
{
    public interface ISupportMessageRepository
    {
        // ── Queries ──────────────────────────────────────────────────────────
        Task<SupportMessage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportMessage>> GetByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportMessage>> GetUnreadByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default);

        // ── Commands ─────────────────────────────────────────────────────────
        Task AddAsync(SupportMessage message, CancellationToken cancellationToken = default);
        Task UpdateAsync(SupportMessage message, CancellationToken cancellationToken = default);
        Task DeleteAsync(SupportMessage message, CancellationToken cancellationToken = default);
    }
}
