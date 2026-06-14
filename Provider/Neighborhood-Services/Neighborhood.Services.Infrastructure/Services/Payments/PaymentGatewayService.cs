using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Neighborhood.Services.Application.Payments.Gateways;
using Neighborhood.Services.Domain.Payments;
using Neighborhood.Services.Infrastructure.Services.Payments.Paymob;

namespace Neighborhood.Services.Infrastructure.Services.Payments
{
    public class PaymentGatewayService : IPaymentGatewayService
    {
        private readonly PaymentGatewayOptions _options;
        private readonly HttpClient _httpClient;

        public PaymentGatewayService(IOptions<PaymentGatewayOptions> options, HttpClient httpClient)
        {
            _options = options.Value;
            _httpClient = httpClient;
        }

        public Task<PaymentGatewayResponse> InitiateAsync(PaymentGatewayRequest request, CancellationToken cancellationToken = default)
        {
            return request.Provider switch
            {
                PaymentProvider.Paymob => InitiatePaymobAsync(request, cancellationToken),
                _ => Task.FromResult(new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = $"Payment provider '{request.Provider}' is not configured."
                })
            };
        }

        private async Task<PaymentGatewayResponse> InitiatePaymobAsync(PaymentGatewayRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_options.PaymobApiKey) ||
                string.IsNullOrWhiteSpace(_options.PaymobIntegrationId) ||
                string.IsNullOrWhiteSpace(_options.PaymobIframeId) ||
                string.IsNullOrWhiteSpace(_options.PaymobBaseUrl))
            {
                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = "Paymob gateway is not configured."
                };
            }

            var authResponse = await _httpClient.PostAsJsonAsync(
                $"{_options.PaymobBaseUrl}/auth/tokens",
                new { api_key = _options.PaymobApiKey },
                cancellationToken);

            if (!authResponse.IsSuccessStatusCode)
            {
                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = "Paymob authentication failed."
                };
            }

            var auth = await authResponse.Content.ReadFromJsonAsync<PaymobAuthResponse>(cancellationToken: cancellationToken);
            if (auth is null || string.IsNullOrWhiteSpace(auth.Token))
            {
                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = "Paymob auth token missing."
                };
            }

            var amount = Convert.ToInt32(Math.Round(request.Amount * 100, 0));
            var orderResponse = await _httpClient.PostAsJsonAsync(
                $"{_options.PaymobBaseUrl}/ecommerce/orders",
                new PaymobCreateOrderRequest
                {
                    Auth_Token = auth.Token,
                    Amount = amount,
                    Currency = request.Currency,
                    Merchant_Order_Id = request.MerchantOrderId ?? Guid.NewGuid().ToString()
                },
                cancellationToken);

            if (!orderResponse.IsSuccessStatusCode)
            {
                var errorBody = await orderResponse.Content.ReadAsStringAsync(cancellationToken);
                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = $"Paymob order creation failed ({(int)orderResponse.StatusCode}): {errorBody}"
                };
            }

            var order = await orderResponse.Content.ReadFromJsonAsync<PaymobCreateOrderResponse>(cancellationToken: cancellationToken);
            if (order is null || order.Id == 0)
            {
                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = "Paymob order id missing."
                };
            }

            if (!int.TryParse(_options.PaymobIntegrationId, out var integrationId))
            {
                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = "Paymob integration id is invalid."
                };
            }

            var paymentKeyResponse = await _httpClient.PostAsJsonAsync(
                $"{_options.PaymobBaseUrl}/acceptance/payment_keys",
                new PaymobPaymentKeyRequest
                {
                    Auth_Token = auth.Token,
                    Amount = amount,
                    Currency = request.Currency,
                    Order_Id = order.Id,
                    Integration_Id = integrationId
                },
                cancellationToken);

            if (!paymentKeyResponse.IsSuccessStatusCode)
            {
                var body = await paymentKeyResponse.Content.ReadAsStringAsync(cancellationToken);
                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = $"Paymob payment key creation failed ({(int)paymentKeyResponse.StatusCode}): {body}"
                };
            }

            var paymentKey = await paymentKeyResponse.Content.ReadFromJsonAsync<PaymobPaymentKeyResponse>(cancellationToken: cancellationToken);
            if (paymentKey is null || string.IsNullOrWhiteSpace(paymentKey.Token))
            {
                return new PaymentGatewayResponse
                {
                    Success = false,
                    ErrorMessage = "Paymob payment token missing."
                };
            }

            if (!string.IsNullOrWhiteSpace(request.Token))
            {
                var payResponse = await _httpClient.PostAsJsonAsync(
                    $"{_options.PaymobBaseUrl}/acceptance/payments/pay",
                    new
                    {
                        source = new { identifier = request.Token, subtype = "TOKEN" },
                        payment_token = paymentKey.Token
                    },
                    cancellationToken);

                if (payResponse.IsSuccessStatusCode)
                {
                    var payResultString = await payResponse.Content.ReadAsStringAsync(cancellationToken);
                    using var doc = System.Text.Json.JsonDocument.Parse(payResultString);
                    bool isSuccess = false;
                    if (doc.RootElement.TryGetProperty("success", out var successProp))
                    {
                        isSuccess = successProp.GetBoolean();
                    }

                    return new PaymentGatewayResponse
                    {
                        Success = true,
                        IsInstant = true,
                        ProviderReference = order.Id.ToString(),
                        RedirectUrl = $"?merchant_order_id={order.Id}&success={(isSuccess ? "true" : "false")}"
                    };
                }
            }

            return new PaymentGatewayResponse
            {
                Success = true,
                ProviderReference = order.Id.ToString(),
                RedirectUrl = $"{_options.PaymobBaseUrl}/acceptance/iframes/{_options.PaymobIframeId}?payment_token={paymentKey.Token}"
            };
        }
    }
}
