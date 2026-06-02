using MediatR;
using Neighborhood.Services.Application.Payments.DTOs;
using Neighborhood.Services.Application.Payments.Interfaces;
namespace Neighborhood.Services.Application.Payments.Queries.GetPaymentMethodsByUserId
{
    public class GetPaymentMethodsByUserIdQueryHandler : IRequestHandler<GetPaymentMethodsByUserIdQuery, IEnumerable<PaymentMethodResponseDto>>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentMethodsByUserIdQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<PaymentMethodResponseDto>> Handle(GetPaymentMethodsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var paymentMethods = await _paymentRepository.GetByUserIdAsync(request.UserId);

            return paymentMethods.Select(pm => new PaymentMethodResponseDto
            {
                Id = pm.Id,
                UserId = pm.UserId,
                PaymentType = pm.PaymentType,
                PaymentProvider = pm.PaymentProvider,
                LastFourDigits = pm.LastFourDigits,
                ExpiryMonth = pm.ExpiryMonth,
                ExpiryYear = pm.ExpiryYear,
                CreatedAt = pm.CreatedAt,
            });
        }
    }
}