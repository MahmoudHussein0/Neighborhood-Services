using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Authorization;
using Neighborhood.Services.Application.Disputes.Commands;
using Neighborhood.Services.Application.Disputes.Queries;
using Neighborhood.Services.Domain.Disputes;
using Neighborhood.Services.Domain.Staffs;

namespace Neighborhood.Services.API.Controllers.Disputes
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    //[HasPermission(PermissionType.ManageDisputes)]
    public class DisputesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DisputesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/disputes
        [HttpGet]
        [HasPermission(PermissionType.ManageDisputes)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllDisputesQuery(), cancellationToken);
            return Ok(result);
        }

        // GET api/disputes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDisputeByIdQuery(id), cancellationToken);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus( DisputeStatus status,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetDisputesByStatusQuery
                {
                    Status = status
                },
                cancellationToken);

            return Ok(result);
        }
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(DisputeType type, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetDisputesByTypeQuery
                {
                    Type = type
                },
                cancellationToken);

            return Ok(result);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser( string userId,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetDisputesByUserQuery
                {
                    UserId = userId
                },
                cancellationToken);

            return Ok(result);
        }
        // POST api/disputes
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDisputeCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT api/disputes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDisputeCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        // DELETE api/disputes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteDisputeCommand { Id = id }, cancellationToken);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
