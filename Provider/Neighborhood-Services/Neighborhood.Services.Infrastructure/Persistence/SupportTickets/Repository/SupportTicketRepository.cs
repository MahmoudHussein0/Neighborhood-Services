using Microsoft.EntityFrameworkCore;
using Neighborhood.Services.Application.SupportTickets.Interfaces;
using Neighborhood.Services.Domain.SupportTickets;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;

namespace Neighborhood.Services.Infrastructure.Persistence.SupportTickets.Repository
{
    public class SupportTicketRepository : GenericRepository<SupportTicket, int>, ISupportTicketRepository
    {
        private readonly ApplicationDbContext _context;

        public SupportTicketRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // ── Queries ──────────────────────────────────────────────────────────



        public async Task<SupportTicket?> GetByIdWithMessagesAsync(
      int id,
      CancellationToken cancellationToken = default)
        {
            return await _context.SupportTickets
                .Include(t => t.Messages)
                .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(
                    t => t.Id == id,
                    cancellationToken);
        }

       

        public async Task<IReadOnlyList<SupportTicket>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
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

        public async Task<IReadOnlyList<SupportTicket>> GetByPriorityAsync(SupportTicketPriority priority, CancellationToken cancellationToken = default)
        {
            return await _context.SupportTickets
                .Where(t => t.Priority == priority)
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
           
        }

        public  Task UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken = default)
        {
            _context.SupportTickets.Update(ticket);
          return Task.CompletedTask;
        }

        public  Task DeleteAsync(SupportTicket ticket, CancellationToken cancellationToken = default)
        {
            ticket.IsDeleted=true;
            _context.SupportTickets.Update(ticket);
            return Task.CompletedTask;

        }
    }
}
