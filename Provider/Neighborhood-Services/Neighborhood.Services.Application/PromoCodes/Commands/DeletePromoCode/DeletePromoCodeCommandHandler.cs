using MediatR;
using Neighborhood.Services.Application.PromoCodes.Interface;
using Neighborhood.Services.Application.Shared;
namespace Neighborhood.Services.Application.PromoCodes.Commands.DeletePromoCode
{
    public class DeletePromoCodeCommandHandler : IRequestHandler<DeletePromoCodeCommand, bool>
    {
        private readonly IPromoCodeRepository _promoCodeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeletePromoCodeCommandHandler(IPromoCodeRepository promoCodeRepository, IUnitOfWork unitOfWork)
        {
            _promoCodeRepository = promoCodeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeletePromoCodeCommand request, CancellationToken cancellationToken)
        {
            var promoCode = await _promoCodeRepository.GetByIdAsync(request.PromoCodeId)
                ?? throw new KeyNotFoundException($"Promo Code with ID {request.PromoCodeId} not found.");

            promoCode.IsDeleted = true;
            promoCode.IsActive = false;

            await _promoCodeRepository.UpdateAsync(promoCode);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}