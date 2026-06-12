using MediatR;
using Neighborhood.Services.Application.Payments.DTOs;
using Neighborhood.Services.Domain.Payments;
namespace Neighborhood.Services.Application.Payments.Commands.AddPaymentMethod
{
    public class AddPaymentMethodCommand : IRequest<PaymentMethodResponseDto>
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public string? UserId { get; set; }
        
        public PaymentType PaymentType { get; set; }
        public PaymentProvider PaymentProvider { get; set; }
        public string ProviderToken { get; set; } = string.Empty;
        public string? LastFourDigits { get; set; }
        public int? ExpiryMonth { get; set; }
        public int? ExpiryYear { get; set; }
    }
}