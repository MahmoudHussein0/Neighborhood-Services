using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Application.Transactions.Commands.CreateTransaction;
using Neighborhood.Services.Application.Transactions.Queries.GetTransactionByType;
using Neighborhood.Services.Application.Transactions.Queries.GetTransactionsByWalletId;
using Neighborhood.Services.Application.Wallets.Queries.GetWalletByUserId;
using Neighborhood.Services.Domain.Transactions;

namespace Neighborhood.Services.API.Transactions
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public TransactionsController(IMediator mediator, ICurrentUserService currentUser)
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

            var wallet = await _mediator.Send(new GetWalletByUserIdQuery { UserId = userId });
            var result = await _mediator.Send(new GetTransactionsByWalletIdQuery { WalletId = wallet.Id });
            return Ok(result);
        }

        [HttpGet("wallet/{walletId:int}")]
        public async Task<IActionResult> GetByWallet(int walletId)
        {
            var result = await _mediator.Send(new GetTransactionsByWalletIdQuery { WalletId = walletId });
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(TransactionType type)
        {
            var result = await _mediator.Send(new GetTransactionByTypeQuery { Type = type });
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransactionCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
