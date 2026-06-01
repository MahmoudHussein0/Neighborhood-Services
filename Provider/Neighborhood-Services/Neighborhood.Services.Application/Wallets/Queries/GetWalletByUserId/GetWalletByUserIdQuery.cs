using MediatR;
using Neighborhood.Services.Application.Wallets.DTOs;
namespace Neighborhood.Services.Application.Wallets.Queries.GetWalletByUserId
{
    public class GetWalletByUserIdQuery : IRequest<WalletResponseDto>
    {
        public string UserId { get; set; } = null!;
    }
}