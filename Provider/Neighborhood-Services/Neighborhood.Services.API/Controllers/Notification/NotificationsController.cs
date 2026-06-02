using MediatR;
using Microsoft.AspNetCore.Mvc;
using Neighborhood.Services.Application.Notifications.Push_inApp.Commands;

namespace Neighborhood.Services.API.Controllers.Notification
{

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private IMediator _mediator;
        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetAll")]
        //public async Task<ActionResult<List<NotificationPuDto>>> GetAll()
        //{
        //    var result = await _mediator.Send(new GetAllNotificationRequest());
        //    return Ok(result);
        //}

        [HttpPut("MarkAllAsRead")]
        public async Task<IActionResult> MarkAllAsRead(MarkAllAsReadCommandDto command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPut("MarkAsRead/{id:int}")]
        public async Task<IActionResult> MarkAsRead(int id, MarkAsReadCommandDto command)
        {
            command.NotificationId = id;
           
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
