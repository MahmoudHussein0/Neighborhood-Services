using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Chatbot.Commands.SendChatMessage;

namespace Neighborhood.Services.API.Chatbot
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ChatbotController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ChatbotController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
}
