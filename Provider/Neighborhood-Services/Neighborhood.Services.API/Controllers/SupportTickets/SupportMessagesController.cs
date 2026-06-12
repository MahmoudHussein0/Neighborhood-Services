using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.SupportTickets.Commands;
using Neighborhood.Services.Application.SupportTickets.Queries;

namespace Neighborhood.Services.API.Controllers.SupportTickets
{
    [ApiController]
    [Route("api/supporttickets/{ticketId}/messages")] // nested under ticket
    //[Authorize]
    public class SupportMessagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SupportMessagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET api/supporttickets/5/messages
        [HttpGet]
        public async Task<IActionResult> GetAll(int ticketId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllSupportMessagesQuery(ticketId), cancellationToken);
            return Ok(result);
        }

        // GET api/supporttickets/5/messages/10
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int ticketId, int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSupportMessageByIdQuery(id), cancellationToken);
            return Ok(result);
        }

        // POST api/supporttickets/5/messages
        [HttpPost]
        public async Task<IActionResult> Create(int ticketId, [FromBody] CreateSupportMessageCommand command, CancellationToken cancellationToken)
        {
            command.TicketId = ticketId; // taken from route, not from body
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { ticketId, id = result.Id }, result);
        }

        // PUT api/supporttickets/5/messages/10
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int ticketId, int id, [FromBody] UpdateSupportMessageCommand command, CancellationToken cancellationToken)
        {
            command.Id = id;
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        // DELETE api/supporttickets/5/messages/10
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int ticketId, int id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteSupportMessageCommand { Id = id }, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int ticketId,int id,CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new MarkSupportMessageAsReadCommand
                {
                    MessageId = id
                },
                cancellationToken);

            return Ok(result);
        }
    }
}
