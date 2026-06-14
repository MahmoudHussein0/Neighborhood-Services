using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Favorites.Commands;
using Neighborhood.Services.Application.Messages.Commands;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Messages.Queries;
using Neighborhood.Services.Application.Shared;
using Neighborhood.Services.Domain.Bookings;

namespace Neighborhood.Services.API.Controllers.Message
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IMediator _mediator;
        private ICurrentUserService _userService;


        public MessageController(IMediator mediator,ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _userService=currentUser;

        }

        [HttpPost]
        public async Task<ActionResult> Add(CreateMessageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id)
        {
            UpdateMessageCommandDTO command = new UpdateMessageCommandDTO();
            command.MessageId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
       

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            DeleteMessagCommandDTO delete_command = new DeleteMessagCommandDTO();
            delete_command.MessageId = id;
            var result = await _mediator.Send(delete_command);
            return Ok(result);
        }

        //Get All Messages
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var res = await _mediator.Send(new GetAllMssgsQDto());
            return Ok(res);
        }

        //Get By Conversation Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetForConversation(int id)
        {
            var res = await _mediator.Send(new GetAllMssgsOfConvQDto() { id=id});
            return Ok(res);
        }

        //Get By Booking Id
        [HttpGet("booking/{id:int}")]
        public async Task<ActionResult> GetForBooking(int id)
        {
            var res = await _mediator.Send(new GetAllMssgsOfBookingQDto() {id=id});
            return Ok(res);
        }

        [HttpGet("CurrentUserId")]
        public async Task<ActionResult> GetCurrentUserId()
        {
            var result = _userService.UserId;
            return Ok(result);
        }
    }
}
