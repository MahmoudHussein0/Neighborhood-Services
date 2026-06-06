using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Chatbot.Commands.SendChatMessage;
using Neighborhood.Services.Application.Chatbot.Queries.GetMySessions;
using Neighborhood.Services.Application.Chatbot.Queries.GetSessionMessages;

namespace Neighborhood.Services.API.Chatbot
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ChatbotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Send a message — works for guests and logged-in users
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage([FromBody] SendChatMessageCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // List the current user's chat sessions (requires login)
        [HttpGet("sessions")]
        [Authorize]
        public async Task<IActionResult> GetMySessions()
        {
            var result = await _mediator.Send(new GetMySessionsQuery());
            return Ok(result);
        }

        // Get one session with its full message history (requires login + ownership)
        [HttpGet("sessions/{sessionId:int}")]
        [Authorize]
        public async Task<IActionResult> GetSessionMessages(int sessionId)
        {
            var result = await _mediator.Send(new GetSessionMessagesQuery { SessionId = sessionId });
            return Ok(result);
        }
    }
}

