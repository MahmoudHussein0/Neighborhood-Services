using MediatR;
using Neighborhood.Services.Application.PromoCodes.DTOs;
namespace Neighborhood.Services.Application.PromoCodes.Commands.CreatePromoCode
{
    public class CreatePromoCodeCommand : IRequest<PromoCodeResponseDto>
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountPercentage { get; set; }
        public int MaxUses { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}