using Neighborhood.Services.Domain.ApplicationUser;
using Neighborhood.Services.Domain.Shared;
namespace Neighborhood.Services.Domain.Payments
{
    public class PaymentMethod : BaseEntity<int>
    {
        public string UserId { get; set; }
        public PaymentType PaymentType { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string ProviderToken { get; set; } = string.Empty;
        public string? LastFourDigits { get; set; }
        public int? ExpiryMonth {  get; set; }
        public int? ExpiryYear { get; set; }
    }
}