namespace Neighborhood.Services.Application.Wallets.DTOs
{
    public class TopUpWalletDto
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
    }
}