using MediatR;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Wallets.DTOs;
using Neighborhood.Services.Application.Wallets.Interfaces;
using Neighborhood.Services.Domain.Wallets;

namespace Neighborhood.Services.Application.Wallets.Queries.GetWalletByUserId
{
    public class GetWalletByUserIdQueryHandler : IRequestHandler<GetWalletByUserIdQuery, WalletResponseDto>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GetWalletByUserIdQueryHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork)
        {
            _walletRepository = walletRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<WalletResponseDto> Handle(GetWalletByUserIdQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);
            
            // Lazy-create wallet for backwards compatibility with older accounts
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserId = request.UserId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                await _walletRepository.AddAsync(wallet);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

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