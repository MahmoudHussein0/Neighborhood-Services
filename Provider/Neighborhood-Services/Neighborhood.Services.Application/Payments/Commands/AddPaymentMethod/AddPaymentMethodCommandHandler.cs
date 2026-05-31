using MediatR;
using Neighborhood.Services.Application.Payments.DTOs;
using Neighborhood.Services.Application.Payments.Interfaces;
using Neighborhood.Services.Application.Shared;
namespace Neighborhood.Services.Application.Payments.Commands.AddPaymentMethod
{
    public class AddPaymentMethodCommandHandler : IRequestHandler<AddPaymentMethodCommand, PaymentMethodResponseDto>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddPaymentMethodCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentMethodResponseDto> Handle(AddPaymentMethodCommand request, CancellationToken cancellationToken)
        {
            var paymentMethod = new Domain.Payments.PaymentMethod
            {
                UserId = request.UserId,
                PaymentType = request.PaymentType,
                PaymentProvider = request.PaymentProvider,
                ProviderToken = request.ProviderToken,
                LastFourDigits = request.LastFourDigits,
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear
            };

            await _paymentRepository.AddAsync(paymentMethod);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PaymentMethodResponseDto
            {
                Id = paymentMethod.Id,
                UserId = paymentMethod.UserId,
                PaymentType = paymentMethod.PaymentType,
                PaymentProvider = paymentMethod.PaymentProvider,
                LastFourDigits = paymentMethod.LastFourDigits,
                ExpiryMonth = paymentMethod.ExpiryMonth,
                ExpiryYear = paymentMethod.ExpiryYear,
                CreatedAt = paymentMethod.CreatedAt,
            };
        }
    }
}