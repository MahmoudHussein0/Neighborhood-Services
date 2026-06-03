using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Escrows.Commands.CreateEscrow;
using Neighborhood.Services.Application.Escrows.Commands.RefundEscrow;
using Neighborhood.Services.Application.Escrows.Commands.ReleaseEscrow;
using Neighborhood.Services.Application.Escrows.Queries.GetEscrowByBookingId;

namespace Neighborhood.Services.API.Escrows
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EscrowsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EscrowsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("booking/{bookingId:int}")]
        public async Task<IActionResult> GetByBookingId(int bookingId)
        {
            var result = await _mediator.Send(new GetEscrowByBookingIdQuery { BookingId = bookingId });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEscrowCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{escrowId:int}/release")]
        public async Task<IActionResult> Release(int escrowId)
        {
            var result = await _mediator.Send(new ReleaseEscrowCommand { EscrowId = escrowId });
            return Ok(result);
        }

        [HttpPost("{escrowId:int}/refund")]
        public async Task<IActionResult> Refund(int escrowId)
        {
            var result = await _mediator.Send(new RefundEscrowCommand { EscrowId = escrowId });
            return Ok(result);
        }
    }
}
