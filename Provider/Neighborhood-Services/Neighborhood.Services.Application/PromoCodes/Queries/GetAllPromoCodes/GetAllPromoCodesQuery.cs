using MediatR;
using Neighborhood.Services.Application.PromoCodes.DTOs;

namespace Neighborhood.Services.Application.PromoCodes.Queries.GetAllPromoCodes
{
    public class GetAllPromoCodesQuery : IRequest<List<PromoCodeResponseDto>>
    {
    }
}
