using Neighborhood.Services.Domain.Shared;
using Neighborhood.Services.Domain.Wallets;
namespace Neighborhood.Services.Domain.Escrows
{
    public class Escrow : BaseEntity<int>
    {
        public int BookingId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public EscrowStatus Status { get; set; } = EscrowStatus.Held;
        public DateTime HeldAt { get; set; } = DateTime.UtcNow;
        public DateTime? ReleasedAt { get; set; }
        public Wallet Wallet { get; set; } = null!;
    }
}