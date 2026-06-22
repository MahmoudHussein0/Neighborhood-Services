using MediatR;
using Neighborhood.Services.Application.PromoCodes.DTOs;
using Neighborhood.Services.Application.PromoCodes.Interface;

namespace Neighborhood.Services.Application.PromoCodes.Queries.PreviewPromoCode
{
    public class PreviewPromoCodeQueryHandler : IRequestHandler<PreviewPromoCodeQuery, PromoCodePreviewDto>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly IPromoCodeUsageRepository _promoCodeUsageRepository;

        public PreviewPromoCodeQueryHandler(
            IPromoCodeRepository promoCodeRepository,
            IPromoCodeUsageRepository promoCodeUsageRepository)
        {
            _promoCodeRepository = promoCodeRepository;
            _promoCodeUsageRepository = promoCodeUsageRepository;
        }

        public async Task<PromoCodePreviewDto> Handle(PreviewPromoCodeQuery request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            // Valid = exists, active, not expired, global uses left.
            if (string.IsNullOrWhiteSpace(code) || !await _promoCodeRepository.IsValidAsync(code))
                return new PromoCodePreviewDto { IsApplicable = false, Reason = "invalid_or_expired" };

            var promo = await _promoCodeRepository.GetByCodeAsync(code);
            if (promo is null)
                return new PromoCodePreviewDto { IsApplicable = false, Reason = "invalid_or_expired" };

            // Per-user: a code already redeemed by this user can't be applied again (mirrors ApplyPromoCode).
            if (!string.IsNullOrWhiteSpace(request.UserId)
                && await _promoCodeUsageRepository.HasUserUsedPromoAsync(request.UserId, promo.Id))
            {
                return new PromoCodePreviewDto
                {
                    IsApplicable = false,
                    DiscountPercentage = promo.DiscountPercentage,
                    Reason = "already_used"
                };
            }

            return new PromoCodePreviewDto
            {
                IsApplicable = true,
                DiscountPercentage = promo.DiscountPercentage
            };
        }
    }
}
