using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Payments.Gateways;
using Neighborhood.Services.Domain.Payments;

namespace Neighborhood.Services.API.Payments
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;

        public PaymentsController(IPaymentGatewayService paymentGatewayService)
        {
            _paymentGatewayService = paymentGatewayService;
        }

        [HttpPost("initiate")]
        public async Task<IActionResult> Initiate([FromBody] PaymentInitiationRequest request)
        {
            var result = await _paymentGatewayService.InitiateAsync(new PaymentGatewayRequest
            {
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                Provider = request.Provider,
                PaymentMethodId = request.PaymentMethodId,
                WalletId = request.WalletId
            });

            return Ok(result);
        }

        public record PaymentInitiationRequest(
            decimal Amount,
            string Currency,
            string? Description,
            PaymentProvider Provider,
            int? PaymentMethodId,
            int? WalletId);
    }
}
