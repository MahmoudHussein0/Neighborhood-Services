using MediatR;
namespace Neighborhood.Services.Application.Payments.Commands.DeletePaymentMethod
{
    public class DeletePaymentMethodCommand : IRequest<bool>
    {
        public int PaymentMethodId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}