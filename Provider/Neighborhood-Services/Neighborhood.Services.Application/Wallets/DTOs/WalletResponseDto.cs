namespace Neighborhood.Services.Application.Wallets.DTOs
{
    public class WalletResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}