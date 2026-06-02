using MediatR;
using Neighborhood.Services.Application.PromoCodes.Interface;
namespace Neighborhood.Services.Application.PromoCodes.Queries.ValidatePromoCode
{
    public class ValidatePromoCodeQueryHandler : IRequestHandler<ValidatePromoCodeQuery, bool>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;

        public ValidatePromoCodeQueryHandler(IPromoCodeRepository promoCodeRepository)
        {
            _promoCodeRepository = promoCodeRepository;
        }

        public async Task<bool> Handle(ValidatePromoCodeQuery request, CancellationToken cancellationToken)
        => await _promoCodeRepository.IsValidAsync(request.Code);
    }
}