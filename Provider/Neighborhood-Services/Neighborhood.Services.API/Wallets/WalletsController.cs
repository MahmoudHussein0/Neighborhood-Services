using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Wallets.Commands.TopUpWallet;
using Neighborhood.Services.Application.Wallets.Queries.GetWalletByUserId;

namespace Neighborhood.Services.API.Wallets
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public WalletsController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyWallet()
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _mediator.Send(new GetWalletByUserIdQuery { UserId = userId });
            return Ok(result);
        }

        [HttpPost("me/topup")]
        public async Task<IActionResult> TopUp([FromBody] TopUpWalletRequest request)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var wallet = await _mediator.Send(new GetWalletByUserIdQuery { UserId = userId });
            var result = await _mediator.Send(new TopUpWalletCommand
            {
                WalletId = wallet.Id,
                Amount = request.Amount,
                PaymentMethodId = request.PaymentMethodId
            });
            return Ok(result);
        }

        public record TopUpWalletRequest(decimal Amount, int PaymentMethodId);
    }
}
