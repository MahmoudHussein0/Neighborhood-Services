using MediatR;
using Neighborhood.Services.Application.Payments.DTOs;
using Neighborhood.Services.Application.Payments.Interfaces;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Payments;
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
            if (string.IsNullOrWhiteSpace(request.UserId))
                throw new InvalidOperationException("User is required.");

            if (string.IsNullOrWhiteSpace(request.ProviderToken))
                throw new InvalidOperationException("Provider token is required.");

            if (!Enum.IsDefined(request.PaymentType))
                throw new InvalidOperationException("Invalid payment type.");

            if (!Enum.IsDefined(request.PaymentProvider))
                throw new InvalidOperationException("Invalid payment provider.");

            if (request.LastFourDigits is not null &&
                (request.LastFourDigits.Length != 4 || !request.LastFourDigits.All(char.IsDigit)))
                throw new InvalidOperationException("Last four digits must contain exactly 4 digits.");

            if (request.ExpiryMonth is not null && (request.ExpiryMonth < 1 || request.ExpiryMonth > 12))
                throw new InvalidOperationException("Expiry month must be between 1 and 12.");

            if (request.ExpiryYear is not null && request.ExpiryYear < DateTime.UtcNow.Year)
                throw new InvalidOperationException("Expiry year cannot be in the past.");

            if (request.ExpiryMonth is not null &&
                request.ExpiryYear is not null &&
                request.ExpiryYear == DateTime.UtcNow.Year &&
                request.ExpiryMonth < DateTime.UtcNow.Month)
                throw new InvalidOperationException("Payment method expiry date cannot be in the past.");

            var paymentMethod = new PaymentMethod
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
