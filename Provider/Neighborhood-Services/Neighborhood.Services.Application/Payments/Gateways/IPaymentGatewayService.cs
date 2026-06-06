namespace Neighborhood.Services.Application.Payments.Gateways
{
    public interface IPaymentGatewayService
    {
        Task<PaymentGatewayResponse> InitiateAsync(PaymentGatewayRequest request, CancellationToken cancellationToken = default);
    }
}