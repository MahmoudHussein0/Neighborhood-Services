using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Interfaces
{
    public interface ISupportMessageRepository : IGenericRepository<SupportMessage, int>
    {
        // ── Queries ──────────────────────────────────────────────────────────
      
        Task<IReadOnlyList<SupportMessage>> GetByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportMessage>> GetUnreadByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default);
        // ── Commands ─────────────────────────────────────────────────────────

        Task DeleteAsync(SupportMessage message, CancellationToken cancellationToken = default);
    }
}
