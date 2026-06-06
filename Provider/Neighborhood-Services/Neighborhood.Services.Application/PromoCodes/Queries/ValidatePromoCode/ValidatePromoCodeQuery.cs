using MediatR;
namespace Neighborhood.Services.Application.PromoCodes.Queries.ValidatePromoCode
{
    public class ValidatePromoCodeQuery : IRequest<bool>
    {
        public string Code { get; set; } = string.Empty;
    }
}