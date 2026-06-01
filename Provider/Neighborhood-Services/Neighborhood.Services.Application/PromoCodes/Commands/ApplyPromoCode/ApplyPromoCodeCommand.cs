using MediatR;
using Neighborhood.Services.Application.PromoCodes.DTOs;
namespace Neighborhood.Services.Application.PromoCodes.Commands.ApplyPromoCode
{
    public class ApplyPromoCodeCommand : IRequest<PromoCodeResponseDto>
    {
        public string Code { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int BookingId { get; set; }
    }
}