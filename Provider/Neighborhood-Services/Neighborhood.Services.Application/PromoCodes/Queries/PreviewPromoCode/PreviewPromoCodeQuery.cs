using MediatR;
using Neighborhood.Services.Application.PromoCodes.DTOs;

namespace Neighborhood.Services.Application.PromoCodes.Queries.PreviewPromoCode
{
    public class PreviewPromoCodeQuery : IRequest<PromoCodePreviewDto>
    {
        public string Code { get; set; } = string.Empty;

        /// <summary>The current user, so the preview can check per-user usage. Set by the controller.</summary>
        public string UserId { get; set; } = string.Empty;
    }
}
