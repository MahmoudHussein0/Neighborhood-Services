using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Authorization;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.Queries;
using Neighborhood.Services.Domain.Staffs;
using Neighborhood.Services.Domain.SupportTickets;

namespace Neighborhood.Services.API.Controllers.SupportTickets
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class SupportTicketsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SupportTicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/supporttickets
        [HttpGet]
        [HasPermission(PermissionType.ManageTickets)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllSupportTicketsQuery(), cancellationToken);
            return Ok(result);
        }

        // GET api/supporttickets/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSupportTicketByIdQuery(id), cancellationToken);
            return Ok(result);
        }
        [HttpGet("{id}/details")]
        [HasPermission(PermissionType.ManageTickets)]
        public async Task<IActionResult> GetDetails(
    int id,
    CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetSupportTicketDetailsQuery
                {
                    Id = id
                },
                cancellationToken);

            return Ok(result);
        }

        // POST api/supporttickets
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSupportTicketCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        //// PUT api/supporttickets/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] updateTicketStatusCommand command, CancellationToken cancellationToken)
        //{
        //    command.Id = id;
        //    var result = await _mediator.Send(command, cancellationToken);
        //    return Ok(result);
        //}
        [HttpPut("{id}/priority")]
        public async Task<IActionResult> UpdatePriority(int id, [FromBody] SupportTicketPriority priority)
        {
            var result = await _mediator.Send(new updateTicketPriorityCommand { Id = id, Priority = priority });
            return Ok(result);
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] SupportTicketStatus status)
        {
            var result = await _mediator.Send(new updateTicketStatusCommand { Id = id, Status = status });
            return Ok(result);
        }

        // DELETE api/supporttickets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteSupportTicketCommand { Id = id }, cancellationToken);
            return Ok(result);
        }
    }

}
