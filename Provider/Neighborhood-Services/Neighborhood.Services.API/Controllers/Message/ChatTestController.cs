using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Messages.DTOs;
using Neighborhood.Services.Application.Shared;


namespace Neighborhood.Services.API.Controllers.Message
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatTestController : ControllerBase
    {
        private IMediator _mediator;
        private IChatService _service;
        private ICurrentUserService _userService;
        public ChatTestController(IMediator mediator, IChatService service, ICurrentUserService userService)
        {
            _mediator = mediator;
            _service = service;
            _userService = userService;
        }

        [HttpPost("SendingToAll")]
        public async Task<ActionResult> CreateNotificationToAll(MessageCreatedDto mssg)
        {
            var result = _service.SendBroadcastMessageDto(mssg);
            return Ok(result);
        }

        [HttpGet("CurrentUserId")]
        public async Task<ActionResult> GetCurrentUserId()
        {
            var result = _userService.UserId;
            return Ok(result);
        }
    }
}
