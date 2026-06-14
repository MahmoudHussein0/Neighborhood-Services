using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Conversations.Commands;
using Neighborhood.Services.Application.Conversations.DTOs;
using Neighborhood.Services.Application.Conversations.Queries;
using Neighborhood.Services.Domain.Bookings;
using Microsoft.AspNetCore.Hosting;


namespace Neighborhood.Services.API.Controllers.Conversation
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        

        private IMediator _mediator;
        public ConversationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //Create Conversation
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateConversationCommandDTO command)
        {
            var result = await _mediator.Send(command);
            if (result == null) { return Ok("error"); }

            return Ok(result);

        }

        //Get all Conversations
        [HttpGet]
        public async Task<ActionResult<List<ConversationSelectedDto>>> GetAllConversations()
        {

            var result = await _mediator.Send(new GetAllConvQDto());
            if (result.Count == 0)
            {
                return Ok("No Conversations yet");

            }
            return Ok(result);
        }

        //Get a Conversation By ID
        [HttpGet("get/{id}")]
        public async Task<ActionResult<ConversationSelectedDto>> GetConvById(int id)
        {
           ConversationSelectedDto? result= await _mediator.Send(new GetByIdConvQDto(){id=id});
            if (result==null) { return Ok($"No Conversations with id {id}"); }
            return result;
        }

        //Get Conversations for current user
        [HttpGet("GetForCurrentUser")]
        public async Task<ActionResult<List<ConversationSelectedDto>>> GetMyConv()
        {
            var result = await _mediator.Send(new GetMyConvsQDto() );
            if (result == null) { return Ok($"No Conversations for this user"); }
            return result;
        }

        //Update a Conversation By ID
        [HttpPost("post/{id}")]
        public async Task<ActionResult<ConversationSelectedDto>> UpdateConvById(int id,[FromBody] UpdateConversationCommandDTO command)
        {
            command.BookingId = id;
            return Ok(await _mediator.Send(command));
        }

        //Delete a Conversation
        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<ConversationSelectedDto>> DeleteConvById(int id, [FromBody] DeleteConversationCommandDTO command)
        {
            command.BookingId = id;
            return Ok(await _mediator.Send(command));
        }

        //Get all messages of a conversation by its ID
        //Get the last message of conversation by its ID



    }
}
