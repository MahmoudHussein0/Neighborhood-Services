namespace Neighborhood.Services.Infrastructure.Services.Payments
{
    public class PaymentGatewayOptions
    {
        public string? PaymobApiKey { get; set; }
        public string? PaymobIntegrationId { get; set; }
        public string? PaymobIframeId { get; set; }
        public string? PaymobHmacSecret { get; set; }
        public string? PaymobBaseUrl { get; set; }

        public string? PayPalClientId { get; set; }
        public string? PayPalClientSecret { get; set; }
        public string? PayPalBaseUrl { get; set; }
    }
}
