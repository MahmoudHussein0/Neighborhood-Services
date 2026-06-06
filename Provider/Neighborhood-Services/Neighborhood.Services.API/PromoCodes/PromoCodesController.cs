using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.PromoCodes.Commands.ApplyPromoCode;
using Neighborhood.Services.Application.PromoCodes.Commands.CreatePromoCode;
using Neighborhood.Services.Application.PromoCodes.Commands.DeletePromoCode;
using Neighborhood.Services.Application.PromoCodes.Queries.GetPromoCodeByCode;
using Neighborhood.Services.Application.PromoCodes.Queries.ValidatePromoCode;
using Neighborhood.Services.Application.Shared;

namespace Neighborhood.Services.API.PromoCodes
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PromoCodesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public PromoCodesController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _mediator.Send(new GetPromoCodeByCodeQuery { Code = code });
            return Ok(result);
        }

        [HttpGet("validate/{code}")]
        public async Task<IActionResult> Validate(string code)
        {
            var result = await _mediator.Send(new ValidatePromoCodeQuery { Code = code });
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePromoCodeCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyPromoCodeCommand command)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            command.UserId = userId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{promoCodeId:int}")]
        public async Task<IActionResult> Delete(int promoCodeId)
        {
            var result = await _mediator.Send(new DeletePromoCodeCommand { PromoCodeId = promoCodeId });
            return Ok(result);
        }
    }
}
