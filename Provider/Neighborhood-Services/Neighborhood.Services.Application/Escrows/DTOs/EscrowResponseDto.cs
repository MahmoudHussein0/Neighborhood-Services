using Neighborhood.Services.Domain.Escrows;
namespace Neighborhood.Services.Application.Escrows.DTOs
{
    public class EscrowResponseDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public EscrowStatus Status { get; set; }
        public DateTime HeldAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
    }
}