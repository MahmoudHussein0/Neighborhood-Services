using MediatR;
namespace Neighborhood.Services.Application.Payments.Commands.DeletePaymentMethod
{
    public class DeletePaymentMethodCommand : IRequest<bool>
    {
        public int PaymentMethodId { get; set; }
    }
}