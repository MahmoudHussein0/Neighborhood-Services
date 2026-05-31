using Neighborhood.Services.Domain.Payments;

namespace Neighborhood.Services.Application.Payments.DTOs
{
    public class PaymentMethodResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public PaymentType PaymentType { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public string? LastFourDigits { get; set; }
        public int? ExpiryMonth { get; set; }
        public int? ExpiryYear { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}