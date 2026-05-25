using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Domain.SupportTickets;

using System;
using System.Collections.Generic;
using System.Text;

namespace Neighborhood.Services.Infrastructure.Persistence.SupportTickets.Repository
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly IApplicationDbContext _context;

        public SupportTicketRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        // ── Queries ──────────────────────────────────────────────────────────

        public async Task<SupportTicket?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.SupportTickets
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<SupportTicket?> GetByIdWithMessagesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.SupportTickets
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<SupportTicket>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SupportTickets
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SupportTicket>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.SupportTickets
                .Where(t => t.UserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SupportTicket>> GetByStatusAsync(SupportTicketStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.SupportTickets
                .Where(t => t.Status == status)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<SupportTicket>> GetByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            return await _context.SupportTickets
                .Where(t => t.BookingId == bookingId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // ── Commands ─────────────────────────────────────────────────────────

        public async Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken = default)
        {
            await _context.SupportTickets.AddAsync(ticket, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken = default)
        {
            _context.SupportTickets.Update(ticket);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(SupportTicket ticket, CancellationToken cancellationToken = default)
        {
            _context.SupportTickets.Remove(ticket);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
