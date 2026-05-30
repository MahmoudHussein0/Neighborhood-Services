using Neighborhood.Services.Application.Escrows.Interfaces;
using Neighborhood.Services.Domain.Escrows;
using Neighborhood.Services.Infrastructure.Persistence.Context;
using Neighborhood.Services.Infrastructure.Shared;
namespace Neighborhood.Services.Infrastructure.Persistence.Escrows
{
    public class EscrowRepository : GenericRepository<Escrow, int>, IEscrowRepository
    {
        public EscrowRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Escrow?> GetByBookingIdAsync(int bookingId)
        {
            var res = await GetByConditionAsync(e => e.BookingId == bookingId);
            return res.FirstOrDefault();
        }

        public async Task RefundAsync(int escrowId)
        {
            var escrow = await GetByIdAsync(escrowId);
            if (escrow is null) return;

            escrow.Status = EscrowStatus.Refunded;
            escrow.ReleasedAt = DateTime.UtcNow;
            await UpdateAsync(escrow);
        }

        public async Task ReleaseAsync(int escrowId)
        {
            var escrow = await GetByIdAsync(escrowId);
            if (escrow is null) return;

            escrow.Status = EscrowStatus.Released;
            escrow.ReleasedAt = DateTime.UtcNow;
            await UpdateAsync(escrow);
        }      
    }
}