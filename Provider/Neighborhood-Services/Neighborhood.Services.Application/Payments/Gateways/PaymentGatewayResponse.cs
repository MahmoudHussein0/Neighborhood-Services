namespace Neighborhood.Services.Application.Payments.Gateways
{
    public class PaymentGatewayResponse
    {
        public bool Success { get; set; }
        public string? RedirectUrl { get; set; }
        public string? ProviderReference { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
