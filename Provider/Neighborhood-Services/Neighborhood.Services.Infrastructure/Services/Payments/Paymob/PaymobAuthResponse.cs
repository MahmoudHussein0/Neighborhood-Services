using System.Text.Json.Serialization;

namespace Neighborhood.Services.Infrastructure.Services.Payments.Paymob
{
    public class PaymobAuthResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
}
