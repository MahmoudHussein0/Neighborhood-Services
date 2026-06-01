using MediatR;
namespace Neighborhood.Services.Application.PromoCodes.Commands.DeletePromoCode
{
    public class DeletePromoCodeCommand : IRequest<bool>
    {
        public int PromoCodeId { get; set; }
    }
}