using MediatR;
using Neighborhood.Services.Application.PromoCodes.DTOs;
using Neighborhood.Services.Application.PromoCodes.Interface;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.PromoCodes;
namespace Neighborhood.Services.Application.PromoCodes.Commands.CreatePromoCode
{
    public class CreatePromoCodeCommandHandler : IRequestHandler<CreatePromoCodeCommand, PromoCodeResponseDto>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePromoCodeCommandHandler(IPromoCodeRepository promoCodeRepository, IUnitOfWork unitOfWork)
        {
            _promoCodeRepository = promoCodeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PromoCodeResponseDto> Handle(CreatePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var normalizedCode = request.Code.Trim();

            if (string.IsNullOrWhiteSpace(normalizedCode))
                throw new InvalidOperationException("Promo code is required.");

            if (request.DiscountPercentage <= 0 || request.DiscountPercentage > 100)
                throw new InvalidOperationException("Discount percentage must be between 0 and 100.");

            if (request.MaxUses <= 0)
                throw new InvalidOperationException("Max uses must be greater than zero.");

            if (request.ExpiresAt <= DateTime.UtcNow)
                throw new InvalidOperationException("Promo code expiration date must be in the future.");

            var exists = await _promoCodeRepository.GetByCodeAsync(normalizedCode);

            if (exists is not null)
                throw new InvalidOperationException($"Promo Code with code {normalizedCode} already exists.");

            var promoCode = new PromoCode
            {
                Code = normalizedCode,
                DiscountPercentage = request.DiscountPercentage,
                MaxUses = request.MaxUses,
                ExpiresAt = request.ExpiresAt,
                IsActive = true,
                UsedCount = 0
            };

            await _promoCodeRepository.AddAsync(promoCode);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PromoCodeResponseDto
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                DiscountPercentage = promoCode.DiscountPercentage,
                MaxUses = promoCode.MaxUses,
                UsedCount = promoCode.UsedCount,
                ExpiresAt = promoCode.ExpiresAt,
                IsActive = promoCode.IsActive,
                CreatedAt = promoCode.CreatedAt,
            };
        }
    }
}
