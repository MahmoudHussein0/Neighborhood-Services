  using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Domain.SupportTickets;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.SupportTickets.Repository
{
    public class SupportMessageRepository : GenericRepository<SupportMessage, int>, ISupportMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public SupportMessageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // ── Queries ──────────────────────────────────────────────────────────

       

        public async Task<IReadOnlyList<SupportMessage>> GetByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            return await _context.SupportMessages
     .Include(m => m.Attachments)
     .Where(m => m.TicketId == ticketId)
     .OrderBy(m => m.CreatedAt)
                 .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // Messages that have not been read yet
        public async Task<IReadOnlyList<SupportMessage>> GetUnreadByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            return await _context.SupportMessages
                .Include(m => m.Attachments)
                .Where(m => m.TicketId == ticketId && m.ReadAt == null)
                .OrderBy(m => m.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // ── Commands ─────────────────────────────────────────────────────────

       

        public  Task DeleteAsync(SupportMessage message, CancellationToken cancellationToken = default)
        {
            // Soft Delete
            message.IsDeleted = true;

            _context.SupportMessages.Update(message);

            return Task.CompletedTask;

        }

     
    }
}
