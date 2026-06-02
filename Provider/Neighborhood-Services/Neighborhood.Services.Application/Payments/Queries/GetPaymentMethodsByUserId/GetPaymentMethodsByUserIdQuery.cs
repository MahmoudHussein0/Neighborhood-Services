using MediatR;
using Neighborhood.Services.Application.Payments.DTOs;
namespace Neighborhood.Services.Application.Payments.Queries.GetPaymentMethodsByUserId
{
    public class GetPaymentMethodsByUserIdQuery : IRequest<IEnumerable<PaymentMethodResponseDto>>
    {
        public string UserId { get; set; } = null!;
    }
}