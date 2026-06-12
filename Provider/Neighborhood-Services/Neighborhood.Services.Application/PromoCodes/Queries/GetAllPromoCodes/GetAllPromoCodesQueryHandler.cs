using MediatR;
using Neighborhood.Services.Application.PromoCodes.DTOs;
using Neighborhood.Services.Application.PromoCodes.Interface;

namespace Neighborhood.Services.Application.PromoCodes.Queries.GetAllPromoCodes
{
    public class GetAllPromoCodesQueryHandler : IRequestHandler<GetAllPromoCodesQuery, List<PromoCodeResponseDto>>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;

        public GetAllPromoCodesQueryHandler(IPromoCodeRepository promoCodeRepository)
        {
            _promoCodeRepository = promoCodeRepository;
        }

        public async Task<List<PromoCodeResponseDto>> Handle(GetAllPromoCodesQuery request, CancellationToken cancellationToken)
        {
            var promoCodes = await _promoCodeRepository.GetAllAsync();
            
            return promoCodes.Select(p => new PromoCodeResponseDto
            {
                Id = p.Id,
                Code = p.Code,
                DiscountPercentage = p.DiscountPercentage,
                MaxUses = p.MaxUses,
                UsedCount = p.UsedCount,
                ExpiresAt = p.ExpiresAt,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            }).ToList();
        }
    }
}
