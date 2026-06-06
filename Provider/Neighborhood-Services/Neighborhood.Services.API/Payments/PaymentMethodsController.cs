using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Payments.Commands.AddPaymentMethod;
using Neighborhood.Services.Application.Payments.Commands.DeletePaymentMethod;
using Neighborhood.Services.Application.Payments.Queries.GetPaymentMethodsByUserId;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.API.Payments
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public PaymentMethodsController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMine()
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetPaymentMethodsByUserIdQuery { UserId = userId });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddPaymentMethodCommand command)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            command.UserId = userId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{paymentMethodId:int}")]
        public async Task<IActionResult> Delete(int paymentMethodId)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _mediator.Send(new DeletePaymentMethodCommand
            {
                PaymentMethodId = paymentMethodId,
                UserId = userId
            });
            return Ok(result);
        }
    }
}
