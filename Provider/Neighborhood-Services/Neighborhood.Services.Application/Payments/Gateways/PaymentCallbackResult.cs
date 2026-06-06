namespace Neighborhood.Services.Application.Payments.Gateways
{
    public class PaymentCallbackResult
    {
        public string? MerchantOrderId { get; set; }
        public string? ProviderReference { get; set; }
        public bool Success { get; set; }
    }
}
