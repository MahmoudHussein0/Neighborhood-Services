using System.Text.Json.Serialization;

namespace Neighborhood.Services.Infrastructure.Services.Payments.Paymob
{
    public class PaymobCreateOrderRequest
    {
        [JsonPropertyName("auth_token")]
        public string Auth_Token { get; set; } = string.Empty;

        [JsonPropertyName("delivery_needed")]
        public bool Delivery_Needed { get; set; } = false;

        [JsonPropertyName("amount_cents")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "EGP";

        [JsonPropertyName("merchant_order_id")]
        public string Merchant_Order_Id { get; set; } = string.Empty;

        [JsonPropertyName("items")]
        public List<object> Items { get; set; } = new();
    }
}
