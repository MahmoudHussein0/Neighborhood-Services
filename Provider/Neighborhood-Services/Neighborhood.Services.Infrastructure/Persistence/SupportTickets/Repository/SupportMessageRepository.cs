using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Domain.SupportTickets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.SupportTickets.Repository
{
    public class SupportMessageRepository : ISupportMessageRepository
    {
        private readonly IApplicationDbContext _context;

        public SupportMessageRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        // ── Queries ──────────────────────────────────────────────────────────

        public async Task<SupportMessage?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.SupportMessages
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<SupportMessage>> GetByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            return await _context.SupportMessages
                .Where(m => m.TicketId == ticketId)
                .OrderBy(m => m.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // Messages that have not been read yet
        public async Task<IReadOnlyList<SupportMessage>> GetUnreadByTicketIdAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            return await _context.SupportMessages
                .Where(m => m.TicketId == ticketId && m.ReadAt == null)
                .OrderBy(m => m.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // ── Commands ─────────────────────────────────────────────────────────

        public async Task AddAsync(SupportMessage message, CancellationToken cancellationToken = default)
        {
            await _context.SupportMessages.AddAsync(message, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(SupportMessage message, CancellationToken cancellationToken = default)
        {
            _context.SupportMessages.Update(message);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(SupportMessage message, CancellationToken cancellationToken = default)
        {
            _context.SupportMessages.Remove(message);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
