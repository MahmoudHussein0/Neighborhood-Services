using System.Text.Json.Serialization;

namespace Neighborhood.Services.Infrastructure.Services.Payments.Paymob
{
    public class PaymobPaymentKeyRequest
    {
        [JsonPropertyName("auth_token")]
        public string Auth_Token { get; set; } = string.Empty;

        [JsonPropertyName("amount_cents")]
        public int Amount { get; set; }

        [JsonPropertyName("expiration")]
        public int Expiration { get; set; } = 3600;

        [JsonPropertyName("order_id")]
        public int Order_Id { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "EGP";

        [JsonPropertyName("integration_id")]
        public int Integration_Id { get; set; }

        [JsonPropertyName("billing_data")]
        public PaymobBillingData Billing_Data { get; set; } = new();
    }

    public class PaymobBillingData
    {
        [JsonPropertyName("first_name")]
        public string First_Name { get; set; } = "Customer";

        [JsonPropertyName("last_name")]
        public string Last_Name { get; set; } = "User";

        [JsonPropertyName("email")]
        public string Email { get; set; } = "customer@example.com";

        [JsonPropertyName("phone_number")]
        public string Phone_Number { get; set; } = "+201000000000";

        [JsonPropertyName("apartment")]
        public string Apartment { get; set; } = "NA";

        [JsonPropertyName("floor")]
        public string Floor { get; set; } = "NA";

        [JsonPropertyName("street")]
        public string Street { get; set; } = "NA";

        [JsonPropertyName("building")]
        public string Building { get; set; } = "NA";

        [JsonPropertyName("shipping_method")]
        public string Shipping_Method { get; set; } = "NA";

        [JsonPropertyName("postal_code")]
        public string Postal_Code { get; set; } = "00000";

        [JsonPropertyName("city")]
        public string City { get; set; } = "Cairo";

        [JsonPropertyName("country")]
        public string Country { get; set; } = "EG";

        [JsonPropertyName("state")]
        public string State { get; set; } = "Cairo";
    }
}
