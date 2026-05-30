using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Escrows;
namespace Neighborhood.Services.Application.Escrows.Interfaces
{
    public interface IEscrowRepository : IGenericRepository<Escrow, int>
    {
        Task<Escrow?> GetByBookingIdAsync(int bookingId);
        Task ReleaseAsync(int escrowId);
        Task RefundAsync(int escrowId);
    }
}