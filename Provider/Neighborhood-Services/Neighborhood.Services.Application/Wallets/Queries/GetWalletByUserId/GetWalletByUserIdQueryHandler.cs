using MediatR;
using Neighborhood.Services.Application.Wallets.DTOs;
using Neighborhood.Services.Application.Wallets.Interfaces;
namespace Neighborhood.Services.Application.Wallets.Queries.GetWalletByUserId
{
    public class GetWalletByUserIdQueryHandler : IRequestHandler<GetWalletByUserIdQuery, WalletResponseDto>
    {
        private readonly IWalletRepository _walletRepository;
        public GetWalletByUserIdQueryHandler(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
        }
        public async Task<WalletResponseDto> Handle(GetWalletByUserIdQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(request.UserId) 
                ?? throw new KeyNotFoundException($"Wallet for user with ID {request.UserId} not found.");

            return new WalletResponseDto
            {
                Id = wallet.Id,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt,
            };
        }
    }
}