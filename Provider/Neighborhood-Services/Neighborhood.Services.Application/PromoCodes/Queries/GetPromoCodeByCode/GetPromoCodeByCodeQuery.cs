using MediatR;
using Neighborhood.Services.Application.PromoCodes.DTOs;
namespace Neighborhood.Services.Application.PromoCodes.Queries.GetPromoCodeByCode
{
    public class GetPromoCodeByCodeQuery : IRequest<PromoCodeResponseDto>
    {
        public string Code { get; set; } = string.Empty;
    }
}