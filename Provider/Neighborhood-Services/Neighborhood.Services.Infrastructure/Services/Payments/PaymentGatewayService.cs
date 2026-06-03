using Microsoft.Extensions.Options;
using Neighborhood.Services.Application.Payments.Gateways;
using Neighborhood.Services.Domain.Payments;

namespace Neighborhood.Services.Infrastructure.Services.Payments
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly PaymentGatewayOptions _options;

        public PaymentGatewayService(IOptions<PaymentGatewayOptions> options)
        {
            _options = options.Value;
        }

        public Task<PaymentGatewayResponse> InitiateAsync(PaymentGatewayRequest request, CancellationToken cancellationToken = default)
        {
            return request.Provider switch
            {
                PaymentProvider.Paymob => Task.FromResult(CreatePaymobStubResponse(request)),
                _ => Task.FromResult(new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = $"Payment provider '{request.Provider}' is not configured."
                })
            };
        }

        private PaymentGatewayResponse CreatePaymobStubResponse(PaymentGatewayRequest request)
        {
            if (string.IsNullOrWhiteSpace(_options.PaymobApiKey) ||
                string.IsNullOrWhiteSpace(_options.PaymobIntegrationId) ||
                string.IsNullOrWhiteSpace(_options.PaymobIframeId))
            {
                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = "Paymob gateway is not configured."
                };
            }

            return new PaymentGatewayResponse
            {
                Success = true,
                ProviderReference = "PAYMOB_STUB",
                RedirectUrl = $"https://accept.paymobsolutions.com/api/acceptance/iframes/{_options.PaymobIframeId}?payment_token=stub"
            };
        }
    }
}
