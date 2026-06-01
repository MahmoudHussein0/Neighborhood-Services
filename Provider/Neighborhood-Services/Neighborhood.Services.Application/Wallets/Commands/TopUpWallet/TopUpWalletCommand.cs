using MediatR;
using Neighborhood.Services.Application.Wallets.DTOs;
namespace Neighborhood.Services.Application.Wallets.Commands.TopUpWallet
{
    public class TopUpWalletCommand : IRequest<WalletResponseDto>
    {
        public int WalletId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
    }
}