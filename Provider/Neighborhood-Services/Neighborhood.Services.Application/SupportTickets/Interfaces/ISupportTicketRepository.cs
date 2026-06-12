using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Application.SupportTickets.Interfaces
{
    public interface ISupportTicketRepository:IGenericRepository<SupportTicket,int>
    {
        // ── Queries ──────────────────────────────────────────────────────────
       
        Task<SupportTicket?> GetByIdWithMessagesAsync(int id, CancellationToken cancellationToken = default);
      
        Task<IReadOnlyList<SupportTicket>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportTicket>> GetByStatusAsync(SupportTicketStatus status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportTicket>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<SupportTicket>> GetByPriorityAsync(SupportTicketPriority priority, CancellationToken cancellationToken = default);

        // ── Commands ─────────────────────────────────────────────────────────

        Task DeleteAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
        Task UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
        Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
        
        }
}
