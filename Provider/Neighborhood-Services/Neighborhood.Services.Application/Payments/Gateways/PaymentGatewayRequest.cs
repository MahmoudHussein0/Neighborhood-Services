using Neighborhood.Services.Domain.Payments;

namespace Neighborhood.Services.Application.Payments.Gateways
{
    public class PaymentGatewayRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string? Description { get; set; }
        public PaymentProvider Provider { get; set; }
        public int? PaymentMethodId { get; set; }
        public int? WalletId { get; set; }
        public string? MerchantOrderId { get; set; }
    }
}
