using MediatR;
using Neighborhood.Services.Application.PromoCodes.DTOs;
using Neighborhood.Services.Application.PromoCodes.Interface;
namespace Neighborhood.Services.Application.PromoCodes.Queries.GetPromoCodeByCode
{
    public class GetPromoCodeByCodeQueryHandler : IRequestHandler<GetPromoCodeByCodeQuery, PromoCodeResponseDto>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;

        public GetPromoCodeByCodeQueryHandler(IPromoCodeRepository promoCodeRepository)
        {
            _promoCodeRepository = promoCodeRepository;
        }

        public async Task<PromoCodeResponseDto> Handle(GetPromoCodeByCodeQuery request, CancellationToken cancellationToken)
        {
            var promoCode = await _promoCodeRepository.GetByCodeAsync(request.Code)
                ?? throw new KeyNotFoundException($"Promo code {request.Code} not found");

            return new PromoCodeResponseDto
            {
                Id = promoCode.Id,
                Code = promoCode.Code,
                DiscountPercentage = promoCode.DiscountPercentage,
                MaxUses = promoCode.MaxUses,
                UsedCount = promoCode.UsedCount,
                ExpiresAt = promoCode.ExpiresAt,
                IsActive = promoCode.IsActive,
                CreatedAt = promoCode.CreatedAt
            };
        }
    }
}