using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Interfaces
{
    public interface ISupportTicketRepository
    {
        // ── Queries ──────────────────────────────────────────────────────────
        Task<SupportTicket?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SupportTicket?> GetByIdWithMessagesAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportTicket>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportTicket>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportTicket>> GetByStatusAsync(SupportTicketStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportTicket>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);

        // ── Commands ─────────────────────────────────────────────────────────
        Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
        Task UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
        Task DeleteAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
    }
}
